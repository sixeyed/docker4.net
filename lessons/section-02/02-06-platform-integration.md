
# Integrating .NET Apps with the Docker Platform

Docker can run any apps in containers. If they don't fit the conventions they still work but you lose the consistency in how you manage apps.

Making apps behave how Docker expects can all be done in the Dockerfile setup, you don't need to change code or redesign apps.


## Understanding app configuration in .NET Core

This is a modern app stack with a new approach to config - reading settings from multiple sources and merging them together.

The default behavior for ASP.NET Core merges in the JSON settings file with environment variables. The reference data API uses a custom setup in [Program.cs](../../src/SignUp.Api.ReferenceData/Program.cs), with sources specified in [ConfigurationBuilderExtensions.cs](../../src/SignUp.Core/Extensions/ConfigurationBuilderExtensions.cs).

_Run a new API container with a custom logging level:_

```
docker ps

docker logs api

docker rm -f api

docker run -d -p 8082:80 --name api `
  --network section-02 `
  -e Logging:LogLevel:Default=Information `
  reference-data-api
```

## Try the new API

The API works in the same way, but the environment variable setting overrides the default logging level.

In this config hierarchy environment variables take precedence, which is the [12-factor](https://12factor.net) approach.

_Make a call to the API and check the logs:_

```
Invoke-RestMethod -Method GET http://localhost:8082/api/roles

docker logs api
```

> The Debug-level logs aren't written any more


## .NET Framework apps are harder

The web app uses the standard XML configuration model. You can use [configuration builders](https://docs.microsoft.com/en-us/aspnet/config-builder) to override app settings from environment variables, but that doesn't work for custom config sections.

The [Web.config](../../src/SignUp.Web/Web.config) file in the web image references the [log4net.config](../../src/SignUp.Web/log4net.config) file which sets up the logging details.

_Find the logs in the web container:_

```
docker exec signup-web powershell cat /logs/signup.log
```

> We need to relay those logs out to Docker


## Tailing the log file 

We saw this in Section 1 with IIS, but it's much simpler with the .NET app - we don't need to configure IIS, we just need the log file relay.

The [v5 Dockerfile](../../docker/02-06-platform-integration/signup-web/v5/Dockerfile) sets that up, using the [start.ps1](../../docker/02-06-platform-integration/signup-web/v5/start.ps1) script to read the log file out to the console.

_Build the new web image:_

```
cd $env:docker4dotnet

docker image build -t signup-web:02-05 `
  -f ./docker/02-06-platform-integration/signup-web/v5/Dockerfile .
```

> The build is fast because almost all of the layers are cached


## Run the new web app

This version also fixes the cold-start issue, because the container makes a local HTTP request in the startup script.

_Replace the web container with a new version:_

```
docker rm -f signup-web

docker run -d -p 8081:80 --name signup-web `
  --network section-02 `
  signup-web:02-05
```

## Try the app again

Now the logs will be collected by Docker. They're still being written by log4net with the same configuration, but the file relay brings them in to Docker.

_Add a new entry to the app at http://localhost:8081/app and check the logs:_

```
docker container exec signup-db `
  powershell `
  "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"

docker logs signup-web
```

> Now there are debug logs


## Tuning the logging level with config

The .NET app only looks to the filesystem for configuration settings. 

We can't override the logging level with environment variables. Instead we can load a different config file into a container.

The [v6 Dockerfile](../../docker/02-06-platform-integration/signup-web/v6/Dockerfile) uses a new [Web.config](../../docker/02-06-platform-integration/signup-web/v6/Web.config) which puts the config files into a separate folder.

_Build the image:_

```
docker image build -t signup-web:02-06 `
  -f ./docker/02-06-platform-integration/signup-web/v6/Dockerfile .
```

## Use a volume mount to override configuration

Docker builds the container filesystem from the image and from other sources. You can mount a local directory on your machine into a directory in the container.

This [log4net.config](../../app/02-06-platform-integration/config/log4net.config) file sets the level to `INFO` - and it can be mounted into the container to override the file from the image.

_Check the default logging config with the custom file:_

```
docker run --entrypoint powershell signup-web:02-06 cat /web-app/config/log4net.config

docker run --entrypoint powershell `
-v "${pwd}\app\02-06-platform-integration\config:C:\web-app\config" `
 signup-web:02-06 `
 cat /web-app/config/log4net.config
```

> When you mount a directory the source replaces all the content in the target directory

## Run a web container with a volume mount

The whole `config` directory gets surfaced from the source of the mount, any files in there from the image are hidden.

The filesystem is a fixed part of the container environment, you can't change the setup for a running container - you need to replace it.

_Remove the existing web container and start a new one:_

```
docker rm -f signup-web

docker run -d -p 8081:80 --name signup-web `
 -v "${pwd}\app\02-06-platform-integration\config:C:\web-app\config" `
 --network section-02 `
 signup-web:02-06
```

## Try the app one last time

The config file is loaded into the container filesystem from the host so log4net is now writing info-level logs.

_Sign up at http://localhost:8081/app and check the logs:_

```
docker container exec signup-db `
  powershell `
  "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"

docker logs signup-web
```

> The app works in the same way, now the logs are at info level


## That's it for now

We'll revisit app configuration and the container filesystem in section 4 when we look at production readiness, but we've got the key ideas now.

Next we'll recap what we've learned in this section.