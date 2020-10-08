# .net core



Docker makes it easy to run features in separate containers, and takes care of communication between containers.

Right now the web application loads reference data direct from the database - that's the list of countries and roles in the dropdown boxes.

We're going to provide that reference data through an API instead.



## The reference data API

The new component is a simple REST API. You can browse the [source for the Reference Data API](./src/SignUp.Api.ReferenceData) - there's one controller to fetch countries, and another to fetch roles.

The API uses a different technology stack:

- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-3.1), a fast cross-platform alternative to full ASP.NET
- [Dapper](https://github.com/StackExchange/Dapper), a fast lightweight ORM

We can use new technologies without impacting the monolith, because this component runs in a separate container.



## Build the API

Check out the [Dockerfile](./docker/backend-rest-api/reference-data-api/Dockerfile) for the API.

It uses the same principle to compile and package the app using containers, but the images use .NET Core running on Nano Server.

_Build the API image:_

```

cd $env:docker4dotnet

docker image build -t reference-data-api `
  -f ./docker/02-05-packaging-dotnet-apps/reference-data-api/Dockerfile .
```



## Run the new API

You can run the API on its own, but it needs to connect to SQL Server.

> appsettings.json

_Run the API, connecting it to the existing SQL container:_

```
docker container ls

docker container run -d -p 8082:80 --name api `
  --network section-02 `
  reference-data-api
```



## Try it out

The API is available on port `8082` on your Docker host. It's a REST API so you can interact with the browser or PowerShell.

_Fetch the list of roles & countries:_

```
Invoke-RestMethod -Method GET http://localhost:8082/api/roles
```

```
Invoke-RestMethod -Method GET http://localhost:8082/api/countries
```

> The response is JSON but PowerShell formats it neatly as a table



- container friendly

```
docker container logs api
```

- get debug level logs

```
docker container logs signup-web
```


no logs at all

