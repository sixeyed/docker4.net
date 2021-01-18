# Packaging Pre-built Applications

You can install pretty much any software into a container image provided it doesn't need UI interaction.

In this lesson we'll package a Windows app from a pre-built MSI into a Docker image.


## Prerequisites

To follow along you will need [Docker Desktop](https://www.docker.com/products/docker-desktop) running in Windows container mode.


## The demo app

This is a simple ASP.NET WebForms app. The MSI has already been built and we can package it with this [Dockerfile](../../docker/02-02-packaging-pre-built-apps/signup-web/v1/Dockerfile).

_Build it in the usual way_:

```
cd $env:docker4dotnet

cd docker/02-02-packaging-pre-built-apps/signup-web/v1

docker image build -t signup-web:02-01 .
```

> The output is an image with the app deployed


## Try the app

The MSI deploys a default set of configuration. 

The app will run but it needs SQL Server so it will error when you try to use it.

_Run a container from the image:_

```
docker run -d -p 8080:80 --name web signup-web:02-01
```

> Browse to the app at http://localhost:8080/signup - after a minute or so you'll see an error message


## Debug the problem

You can connect to the container to check the configuration settings.

_Run an interactive session in the container:_


```
docker exec -it web powershell

ls

cd docker4.net\SignUp.Web

cat web.config

cat .\connectionStrings.config

exit
```

> The app expects to find SQL Server at the network address `SIGNUP-DB-DEV01`


## Run a SQL Server container

Containers can be connected on a Docker network and the container name acts as the machine name.

The Docker Engine has a DNS server which resolves container names to IP addresses.

_Run a SQL container and confirm the web container can reach it:_

```
docker run -d --name SIGNUP-DB-DEV01 `
  --env sa_password=DockerCon!!! `
  docker4dotnet/sql-server:2017

docker exec -it web powershell ping SIGNUP-DB-DEV01
```

## Check the container networks

You can explicitly attach containers to networks. Windows containers connect to the `nat` network by default.

```
docker container inspect web

docker network inspect nat
```

> Docker networks can be used to isolate container workloads


## Try the app now

The app uses Entity Framework code first, so it creates the database schema when it connects to the empty SQL Server instance. 

The website reads reference data fron the database and inserts new data when a user signs up.

_Try the app at http://localhost:8080/signup_

> Click the Sign Up button and enter some details


## Check the database

When you see the _Thank You_ page, the data is in SQL Server.

The database container doesn't publish any ports, but we can run a SQL query inside the container to see the data.

_Query the prospects table in the database:_

```
docker container exec SIGNUP-DB-DEV01 `
  powershell `
  "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"
```

> You'll see the new data you added


## This works, but it's not efficient

Docker images are a packaging and distribution format - and so are MSIs. 

Using an MSI means you need to maintain another packaging setup. This one uses [WiX](https://wixtoolset.org):

- [Product.wxs](../../src/SignUp.Web.Installer/Product.wxs) for the install
- [SignUp.wxs](../../src/SignUp.Web.Installer/SignUp.wxs) for the application content

It also means there are extra stages to the build and your builds require more tools.

We'll look at a different approach next.