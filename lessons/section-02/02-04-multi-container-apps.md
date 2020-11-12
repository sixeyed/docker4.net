
# Running Multi-container Applications

Docker creates the container environment where your applications run.

Every container has an IP address managed by Docker and containers connected to the same Docker network can communicate with standard network protocols.

You'll package your application images with a default set of configuration, including connection details for other components.

## Configuring connections

The web app bundles the database connection string details from source code, but it's better to use separate settings for Docker.

The [v4 Dockerfile](../../docker/02-04-multi-container-apps/signup-web/Dockerfile) copies in a new [connectionStrings.config](../../docker/02-04-multi-container-apps/signup-web/connectionStrings.config) config file with settings better suited to containers.

_Build the new version and check the config:_

```
cd $env:docker4dotnet

docker image build -t signup-web:02-04 `
  -f ./docker/02-04-multi-container-apps/signup-web/Dockerfile .

docker run --rm --entrypoint powershell signup-web:02-04 cat /web-app/connectionStrings.config
```

## Create a custom network for the app

The default `nat` network is fine for development, but for more control you can create dedicated networks for your apps.

_Create a new network and check its details:_

```
docker network ls

docker network create --driver nat section-02

docker network inspect section-02
```

> Each network has it's own gateway address


## Run a new database container

The new network lets us run an isolated instance of the app.

_Run a SQL container connecting to the new network:_

```
docker run -d --name signup-db `
  --env sa_password=docker4.net! `
  --network section-02 `
  docker4dotnet/sql-server:2017

docker container inspect signup-db
```

> The network gateway is from the new Docker network


## Connect an app container to the database

Containers in the same network can communicate using the container name as the machine name.

Docker has a DNS server built in which returns the container IP address for lookups.

_Run a new app container, attached to the new network:_

```
docker run -d -p 8081:80 --name signup-web `
  --network section-02 `
  signup-web:02-04

docker network inspect section-02

docker exec -it signup-web powershell ping signup-db

docker exec -it web powershell ping signup-db
```

> Containers can only connect if they're attached to the same network

## Verify the app works

The new containers make up a separate environment. You can use this approach to run different versions of the app or different configurations.

_Add some new data to the app at http://localhost:8081/app and check the database:_

```
docker container exec signup-db `
  powershell `
  "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"
```

## Connecting containers is easy

Docker makes it simple to build distributed apps where each component runs in a small, lightweight container.

You can add features by deploying new containers without needing to change and regression test a big monolithic app.

Containers are isolated so you can use different technology stacks for different components.



