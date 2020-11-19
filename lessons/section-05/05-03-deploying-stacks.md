# Deploying Application Stacks

Docker Swarm uses the standard Compose specification, with some additional sections to configure Swarm features. Some parts of the spec only apply when you run the app with Compose and others only apply when you run in Swarm - both runtimes ignore sections that aren't relevant.

You deploy applications as **stacks** in Swarm mode. A stack is defined in a single Compose file, and it can contain definitions for services and for other Docker resources - like config objects, secrets and networks.

## Shared components - infrastructure

In a production cluster you can have some components which are shared between multiple apps. The message queue and reverse proxy from the sample application are good candidates - they're defined in the Compose file [infrastructure.yml](../../app/05/05-03/infrastructure.yml).

This is a standard Compose file, with the additional `deploy` section which only applies in Swarm mode. It tells the Swarm that these services should run on Linux nodes in the cluster.

_Create the networks and deploy the stack:_

```
vagrant ssh manager 

docker network create -d overlay frontend-net

docker network create -d overlay backend-net

cd /docker4dotnet

docker stack deploy -c app/05/05-03/infrastructure.yml infrastructure
```

_Check the containers in the stack services:_

```
docker stack ps infrastructure

docker service logs infrastructure_proxy
```

> Any apps can be routed through the proxy if the services attach to the front-end network, and any components can use the message queue if they attach to the back-end network.

## Storing app configuration

The Swarm cluster has a distributed database running in the manager node(s). It's where all the stack and service specifications are stored so you can manage apps without needing the Compose file. It's also the storage layer for application config.

Config objects are Docker resources in Swarm mode. You create them from files, do we can store the [signup-web-appsettings.json](../../app/05/05-03/configs/signup-web-appsettings.json) as a config object and mount it into the container filesystem. Config objects can be any format - JSON, XML or even binary files.

_Create all the config objects for the app components:_

```
docker config create signup-web-appsettings app/05/05-03/configs/signup-web-appsettings.json

docker config create signup-web-log4net app/05/05-03/configs/signup-web-log4net.config

docker config create reference-data-config app/05/05-03/configs/reference-data-config.json
```

_Check the configs and the contents:_

```
docker config ls

docker config inspect --pretty signup-web-log4net

docker config inspect --pretty reference-data-config
```

## Storing sensitive app settings

Anyone with access to the cluster can see the contents of a config object, so you shouldn't use them to store sensitive data like passwords or API keys. Instead you store those settings in **secret** objects, which are encrypted in the Swarm.

You create secrets from a file just like config objects, and they can be mounted into the container filesystem. They're encrypted at rest and in transit - the plain text is only visible inside the container.

_Create secrets for the app's sensitive config settings:_

```
docker secret create signup-db-password app/05/05-03/secrets/signup-db-password

docker secret create signup-web-connectionstrings app/05/05-03/secrets/signup-web-connectionStrings.config

docker secret create save-handler-connectionstrings app/05/05-03/secrets/save-handler-connectionStrings.config

docker secret create reference-data-secret app/05/05-03/secrets/reference-data-secret.json
```

_Check the secrets and the contents:_

```
docker secret ls

docker secret inspect --pretty reference-data-secret
```

> You can't read the contents of a secret in the cluster. The only way to read it is from inside a container which mounts the secret.

## Deploy the main application components

The core parts of the sample application are modelled in [05-03/signup.yml](../../app/05/05-03/signup.yml). There's a mix of Windows and Linux components which will be spread around the nodes.The spec includes labels so the front-end components get found by Trafik, and mounts for the config and secret objects. 

All the images are published on Docker Hub. Swarm nodes have to be able to access images so they need to be pushed to a registry. These have all been built from the Dockerfiles in the `05-03-deploying-stacks` folder - there's nothing new except in the [signup-web/start.ps1](../../docker/05-03-deploying-stacks/signup-web/start.ps1) which has some logic to set up the config files.

_Deploy the application stack:_

```
docker network create -d overlay signup-net

docker stack deploy -c app/05/05-03/signup.yml signup
```

_Check where the stack containers are running:_

```
docker stack ps signup

docker stack ps signup  -f "desired-state=running"

docker service logs signup_save-handler
```

> The message handler on the Windows node can reach the NATS message queue running on a Linux node across the overlay network. Browse to any node at port `8080` and test the app.

_Verify that the data gets saved:_


```
docker service logs signup_signup-web

docker service logs signup_save-handler
```

## Deploy the analytics part of the app

The core app doesn't include the analytics components - those are modelled separately in [05-03/analytics.yml](../../app/05/05-03/analytics.yml). The services connect to whichever networks they need, so you can model one large application as subsystems in different stacks.

_Deploy the analytics stack:_

```
docker network create -d overlay analytics-net

docker stack deploy -c app/05/05-03/analytics.yml analytics

docker stack ps analytics
```

> Test the app again with another sign up

_Verify the message is processed by both handlers:_

```
docker service logs signup_save-handler

docker service logs analytics_index-handler
```

> Browse to any node on port `5601` and check the data in Kibana.

## Modelling production concerns

The app is running now, in a container platform which provides high availability and scale. 

We're not really making use of those features with our simple app definitions, so next we'll extend them to make the application more suitable for production.