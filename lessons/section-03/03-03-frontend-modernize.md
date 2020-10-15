# Modernizing .NET Framework Apps

Monoliths can run in containers just fine. But they aren't modern apps - they're just old apps running in containers.

You can rebuild a monolith into microservices, but that's a long-term project.

We'll do it incrementally instead, by breaking features out of the monolith and running them in separate containers - starting with the reference data API and the app's homepage.

## Integrating the reference data API

The WebForms app needs code changes to fetch reference data from the new API rather than from the database.

I've done that by bringing in the same dependency injection and configuration approach used in .NET Core apps:

- [Global.asax.cs](../../src/SignUp.Web/Global.asax.cs) sets up the config and services
- [appsettings.json](../../src/SignUp.Web/appsettings.json) is for the new config settings
- [SignUp.aspx.cs](../../src/SignUp.Web/SignUp.aspx.cs) uses config to load dependencies
- [ApiReferenceDataLoader.cs](../../src/SignUp.Web/ReferenceData/ApiReferenceDataLoader.cs) is the API consumer

## Using configuration for feature switches

This code is all in the `:02-06` web image we've already built, and the new features can be enabled with configuration.

Compose spec [v3.yml](../../app/03/v3.yml) sets the new config for the web app:

- using environment variables to set the reference data loader implementation
- removing the volume bind so we see all the debug level logs


## Upgrade the web app

Changes to configuration settings mean changes to the container environment, so the existing web container will be replaced.

_Update the app to the new definition and check the logs:_

```
docker-compose -f v3.yml up -d

docker logs -f 03_signup-web_1

docker logs 03_reference-data-api_1
```

> The `-f` flag follows the logs, so Docker will watch for new container logs.

## Replacing the app homepage

The current homepage is in the WebForms codebase. Any UI changes would need a complete rebuild of the monolith.

Check out [the new homepage](../../docker/03-03-frontend-modernize/homepage/index.html). It's a static HTML site which uses Vue.js - it will run in its own container, so it can use a different technology stack from the main app.

The [Dockerfile](../../docker/03-03-frontend-modernize/homepage/Dockerfile) is really simple - it just copies the HTML content into an IIS image.

_Build the homepage image:_

```
cd $env:docker4dotnet

docker image build `
  -t homepage `
  -f ./docker/03-03-frontend-modernize/homepage/Dockerfile .
```

## Run the new homepage

You can run the homepage on its own - great for fast iterating through changes.

_Run the homepage and try it out:_

```
docker container run -d -p 8040:80 --name home homepage
```

> Browse to http://localhost:8040


## Almost there

The new homepage looks good, starts quickly and is packaged in a small Windows Server Core image.

It doesn't work on its own though - click _Sign Up_ and you'll get an error.

To use the new homepage **without changing the original app** we can run a reverse proxy in another container.
