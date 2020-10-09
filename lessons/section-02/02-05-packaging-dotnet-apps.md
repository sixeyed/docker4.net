# Compiling and Packaging .NET Core Apps

Docker makes it easy to run features in separate containers, and takes care of communication between containers.

Right now the web application loads reference data direct from the database - that's the list of countries and roles in the dropdown boxes.

We're going to provide that reference data through an API.


## The reference data API

The new component is a simple ASP.NET Core Web API. There's one controller to fetch [countries](../../src/SignUp.Api.ReferenceData/Controllers/CountriesController.cs), and another to fetch [roles](../../src/SignUp.Api.ReferenceData/Controllers/RolesController.cs).

The API uses [Dapper](https://github.com/StackExchange/Dapper) for data access. It's a fast lightweight ORM - the API doesn't need the full features of Entity Framework.

## Build the API

Check out the [Dockerfile](../../docker/02-05-packaging-dotnet-apps/reference-data-api/Dockerfile) for the API.

It uses the same multi-stage Dockerfile approach to compile and package the app using containers. The SDK and runtime images are the multi-arch .NET Core variants.

_Build the API image:_

```
cd $env:docker4dotnet

docker image build -t reference-data-api `
  -f ./docker/02-05-packaging-dotnet-apps/reference-data-api/Dockerfile .
```

## Run the new API

The API can use the same SQL Server database as the web application.

The Docker image packages the [appsettings.json](../../docker/02-05-packaging-dotnet-apps/reference-data-api/appsettings.json) configuration file which uses the `signup-db` database name.

_Run the API, attaching it to the app network:_

```
docker container ls

docker container run -d -p 8082:80 --name api `
  --network section-02 `
  reference-data-api
```

## Try it out

The API is available on port `8082` on your Docker host. It's a REST API so you can cal it from the browser or with PowerShell.

_Fetch the lists of roles & countries:_

```
Invoke-RestMethod -Method GET http://localhost:8082/api/roles

Invoke-RestMethod -Method GET http://localhost:8082/api/countries
```

> The response is JSON but PowerShell formats it neatly as a table


## Check the API logs

.NET Core is container-friendly. The entrypoint is the `dotnet` process which runs in the foreground. 

The app is configured to write logs to the console, which where Docker looks for them. 

_Show the API logs:_

```
docker container logs api
```

> Logging is set to Debug level in the packaged config file


## Compare the web container

The web application is ASP.NET Framework running in IIS, which is a background process in a Windows Service. 

The app writes logs to a file but Docker doesn't see them.

_Show the web container logs:_

```
docker container logs signup-web
```

> Nothing


## Apps need to be good citizens for Docker

Docker adds consistency to all your apps but apps need to behave in the expected way to get the full benefit.

As a minimum you should set up your Docker images so containers can read configuration settings from the environment and write logs to standard out.

Next we'll see how that works for the .NET Framework and .NET Core components.

