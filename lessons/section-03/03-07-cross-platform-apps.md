# Running Cross Platform Distributed Apps

Parts of our app could run in Linux containers, which is a a more lightweight option (and cheaper to run in cloud environments).

In producrion we'd use a container platform with some Windows servers and some Linux servers, but in development it's easier not to manage multiple VMs. 

You can run a hybrid solution using Docker Desktop, but there are some limitations.


## Modelling the Windows components

Docker Compose specs can include `build` sections which means you can use the Compose command line to build all the images for your app.

The [v7-windows.yml](../../app/03/v7-windows.yml) spec includes just the Windows and front-end components, with build details for all the custom images.

_Build the images with Compose:_

```
cd $env:docker4dotnet

docker-compose -f ./app/03/v7-windows.yml build
```

> The build should be fast because all the layers are cached.


## Clear down the existing app

Remove all the Windows containers, we'll deploy the new specs later in the lesson.

```
docker container rm -f $(docker container ls -aq)
```

## Prep the Linux image variants

The [v7-linux.yml](../../app/03/v7-linux.yml) spec includes the .NET Core analytics message handler, and official images for NATS, Elasticsearch and Kibana.

The message queue will be used by the WebForms Windows container, so the port is published to the host.

_Switch to Linux container mode and build and pull the images:_

```
docker-compose -f ./app/03/v7-linux.yml build

docker-compose -f ./app/03/v7-linux.yml pull
```

## Run the Linux components

Clear down any existing containers and use Compose to run the Linux part of the stack.

_We're in Linux container mode now:_

```
docker container rm -f $(docker container ls -aq)

docker-compose -f ./app/03/v7-linux.yml up -d
```

## Check the Linux components are running

You'll see the infrastructure containers and the message handler running in Linux. 

They're connected to the `03_signup-net` network on the Linux Docker server.

_Check the containers and the logs:_

```
docker ps

docker logs 03_signup-index-handler_1
```

## Run the Windows components

We'll use Windows containers for the .NET Framework components, together with SQL Server and Traefik. The [v7-windows.yml](../../app/03/v7-windows.yml) configuration routes to the message queue via the host machine's IP address.

We could run the API and homepage as Linux containers, but Traefik wouldn't find them - it can only connect to one Docker server.

_Switch to Windows container mode, set up networking and run the containers:_

```
Set-NetFirewallProfile -Profile Domain,Public,Private -Enabled False

$env:HOST_IP=(Get-NetIPConfiguration | Where-Object {$_.IPv4DefaultGateway -ne $null})[0].IPv4Address.IPAddress

echo $env:HOST_IP

docker-compose -f ./app/03/v7-windows.yml up -d
```

## Try the app

Windows and Linux containers are running on different Docker servers, so they communicate over published ports.

The app still works in the same way, and this is a useful way to test out Linux versions of your components.

_Add some new data at http://localhost:8080 and check it makes it to the databases:_

```
docker container exec 03_signup-db_1 powershell `
 "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"
```

> Browse to http://localhost:5601 and configure the index - you'll see the data from Elasticsearch


## This is just a handy workaround

It's clunky to publish ports when you don't need to, and to switch between Docker servers to manage your containers. And you have to use separate Docker Compose files which means your local deployment is different from other environments.

In section 5 we'll see how to run this same app using a container platform with multiple servers. We'll use local VMs for that, but the approach would be the same in the cloud.