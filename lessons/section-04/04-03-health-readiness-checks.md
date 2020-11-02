# Health and Readiness Checks

Docker monitors the startup process in the container and if that process ends then the container exits. This is a basic liveness check which doesn't ensure the app is actually working - a web server could still be running but returning `503` responses to every request because it's maxed out.

You can add a healthcheck to container images so Docker tests your app and makes sure it actually is healthy. We'll see some different ways to approach that in this lesson.

The other side of health is a readiness check which confirms your app is actually able to start work. You need that for apps which don't fail gracefully if they can't reach dependencies, and we'll see how to build that logic into the container startup command.


## Healthchecks for web apps

This [Dockerfile](../../docker/04-03-health-readiness-checks/signup-web/Dockerfile) for the ASP.NET app adds a `HEALTHCHECK` instruction. Docker will run the healthcheck command inside the container, and use the exit code to determine the container health - `0` means healthy.

For the new image there's a Docker Compose override file in [signup-web.yml](../../app/04/04-03/signup-web.yml) with the build details and a new image tag.

_Join the two Compose files to see the combined configuration:_

```
cd $env:docker4dotnet

docker-compose -f app/04/web.yml -f app/04/04-03/signup-web.yml config
```

## Build the web app image

[Override files](https://docs.docker.com/compose/extends/) are a great way to manage your app defintions with Compose, so they don't get bloated. You can separate build details from runtime config, and have different settings for different environments.

_Build and run the updated web app with the healthcheck:_

```
docker-compose -f app/04/web.yml -f app/04/04-03/signup-web.yml build signup-web

docker-compose -f app/04/web.yml -f app/04/04-03/signup-web.yml up -d

docker ps
```

> You'll see the web container has a different status from the others - `health: starting`

## Verify the healthcheck

Docker is running the `Invoke-WebRequest` command inside the container every five seconds. If the response is not a 200-OK then the healthcheck fails - three consecutive failures means the container is flagged as unhealthy.

_View the healthcheck status for the container:_

```
docker inspect 04_signup-web_1
```

> Unhealthy containers are left running on a Docker server, but a container platform would replace them.

## Healthchecks with a custom utility

`Invoke-WebRequest` is fine for a simple healthcheck in a web app but it requires PowerShell in the container image, so it's not suitable for .NET Core apps which could run on Nano Server or Linux.

Writing a custom healthcheck utility means you can use more complex logic, make it cross-platform and remove the need for additional tools in your container image.

The new [Dockerfile](../../docker/04-03-health-readiness-checks/reference-data-api/Dockerfile) for the reference data API does that, with a utility that could be used for other components too. The utility code is in the [Program](../../src/Utilities.HttpCheck/Program.cs) class.


## Build and run the new API

The new API image tag is in the [reference-data-api.yml](../../app/04/04-03/reference-data-api.yml) override file with the build details.

_Print the full Compose config and then update the API:_

```
docker-compose -f app/04/web.yml -f app/04/04-03/reference-data-api.yml config

docker-compose -f app/04/web.yml -f app/04/04-03/reference-data-api.yml build reference-data-api

docker-compose -f app/04/web.yml -f app/04/04-03/reference-data-api.yml up -d reference-data-api
```

## Check the API status

Healthchecks have the same UX whatever the command inside the container is doing. You can check on the API container in the same way as the web app container.

We're using Windows containers but you can build the API in Linux container mode and the healthcheck will work in the same way.

_Inspect the healthcheck results for the API:_

```
docker ps

docker logs 04_reference-data-api_1

docker inspect 04_reference-data-api_1
```

## Readiness checks

Healthchecks make sure your app is working correctly, and some apps will also need a **readiness check** to make sure they are able to start work.

Apps might be built with the assumption that all dependencies are available, and not be resilient to failure. That's where you need a readiness check because you don't want a container which looks healthy but where the app is unable to do anything.

The save handler is a good example. It needs to connect to the message queue but in the [Program](../../src/SignUp.MessageHandlers.SaveProspect/Program.cs) class it silently fails if the queue is not available.

## Understanding the need for readiness checks

If there's no message queue then the save handler can't do any work. In the current state the application process keeps running so the container stays up and Docker thinks everything is OK.

_Stop the queue and restart the message handler:_

```
docker container stop 04_message-queue_1

docker container restart 04_signup-save-handler_1

docker ps

docker container logs 04_signup-save-handler_1 --tail 2
```

> The app is broken but the container is up. A container platform doesn't know to take corrective action.

## Build a handler image with a readiness check

Docker doesn't have built-in support for readiness, but you can add a check in the startup command. The new [Dockerfile](../../docker/04-03-health-readiness-checks/save-handler/Dockerfile) for the message handler does that.

It uses a utility to check the message queue is available (all done in the [Program](../../src/Utilities.MessageQueueCheck/Program.cs) class), and if it's not then the container exits.

_Build the new image and replace the handler:_

```
docker-compose -f app/04/web.yml -f app/04/04-03/save-handler.yml build signup-save-handler

docker-compose -f app/04/web.yml -f app/04/04-03/save-handler.yml up -d signup-save-handler
```

> The dependencies in the Compose spec mean the queue is restarted too.

## Verify the readiness check

Repeat the test and you'll see that the handler exits if it can't access the message queue on startup.

_See how the handler behaves with the readiness check:_

```
docker container logs 04_signup-save-handler_1

docker container stop 04_message-queue_1

docker container restart 04_signup-save-handler_1

docker ps -a

docker container logs 04_signup-save-handler_1
```

> It's counter-intuitive but you want containers to fail fast if they can't do any work.

## Empowering the container platform

Health checks and readiness checks don't do much on a single Docker server, but they'll help your apps to be self-healing in production. A container platform will restart or replace containers which have exited or are unhealthy.

You'll also run a monitoring solution inside your container platform to see how your apps are performing and get early notice when there are problems. For that you need to add monitoring to your apps, which we'll cover next.