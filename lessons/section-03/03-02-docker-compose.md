# Modelling and Running Apps with Docker Compose

[Docker Compose](https://docs.docker.com/compose/) is a separate part of the Docker ecosystem. You use the Compose specification to model your applications in a YAML file, and the `docker-compose` command line to run apps from the YAML spec.

It's how you define a multi-container app so you don't need to manually create containers and networks.

## Understanding Docker Compose

The Compose command line is installed with Docker Desktop, so you don't need a separate setup.

_Check the version of Compose and the commands it supports:_

```
docker-compose version

docker-compose
```

> The Compose command line uses the Docker API, just like the `docker` command line.

## Modelling apps with Compose

In section 2 we ran a SQL Server database, ASP.NET web app and ASP.NET Core web API in containers.

This [docker-compose.yml](../../app/03/docker-compose.yml) spec describes the same app:

- the `services` section defines the parts of the app which run in containers
- the `networks` section defines the Docker networks

> This is a declarative approach rather than the imperative way of running `docker` commands. The Compose file is often called the _application manifest_.

## Running apps with Compose

The Compose file has the complete structure of the app, and you can start it with a single command.

_Switch to the directory and start the app:_

```
docker ps

cd app/03

docker-compose up -d
```

## Check the components

Compose reads the desired state in the YAML file and compares it with what's actually running in Docker. 

It creates or replaces objects to get from the current state to the desired state.

_Look at the Docker objects created by the Compose command:_

```
docker container ls

docker network ls
```

## Test the web app

The app works in exactly the same way, we've just moved the management overhead into the Docker Compose file.

_Sign up at http://localhost:8081/app and check the database:_

```
docker container exec 03_signup-db_1 `
  powershell `
  "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"
```

## Test the API

The Compose spec includes the reference data API.

The API and web containers use the default config settings in the Docker images.

_Try the API and check the container logs:_

```
Invoke-RestMethod -Method GET http://localhost:8082/api/roles

docker logs 03_signup-web_1

docker logs 03_reference-data-api_1
```

## Configuring logging

The Compose spec includes all the options you would usually use in `docker` commands, so you can define environment variables and volume mounts too.

The [v2.yml](../../app/03/v2.yml) Compose spec adds logging configuration to set logging level to Info.

_Bring up the new version of the app:_

```
docker-compose -f v2.yml up -d

docker ps
```

> Compose replaces containers where the definition has changed.

## Confirm the new logging configuration

The new containers use the same networking setup, so you can use them in the same way.

_Make some HTTP calls and check the logs:_

```
curl http://localhost:8081/app/

docker logs 03_signup-web_1

curl http://localhost:8082/api/countries

docker logs 03_reference-data-api_1
```

## The platform is modern but...

All the logic is in the old WebForms app. It's a monolith which is hard to maintain and difficult to extend.

Next we'll start the process of modernizing the app without a full rewrite.

