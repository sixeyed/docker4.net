# Adding Features with Message Handlers

The app uses SQL Server for storage, which isn't very friendly for business users to get reports. Next we'll add self-service analytics, using enterprise-grade open-source software.

We'll be running [Elasticsearch](https://www.elastic.co/products/elasticsearch) for storage and [Kibana](https://www.elastic.co/products/kibana) to provide an analytics front-end.


## Pub-sub messaging

To get data into Elasticsearch when a user signs up, we just need another message handler, which will listen for the same messages published by the web app.

The new handler is a .NET Core console application. The code is in [QueueWorker.cs](../../src/SignUp.MessageHandlers.IndexProspect/Workers/QueueWorker.cs) - it subscribes to the same event messages, then enriches the data and stores it in Elasticsearch.


## Build the analytics message handler

The new message handler only shares the messaging library with the WebForms app, so there are no .NET Framework dependencies and it can run in .NET Core.

The [Dockerfile](../../docker/03-06-backend-analytics/index-handler/Dockerfile) follows the familiar pattern - the app image is based on the .NET Core runtime image.

_Build the image in the usual way:_

```
docker image build --tag index-handler `
  --file ./docker/03-06-backend-analytics/index-handler/Dockerfile .
```

## Running Elasticsearch in Docker

The Elasticsearch team maintain their own Docker image for Linux containers, but not yet for Windows.

It's easy to package your own image to run Elasticsearch - this [Dockerfile](../../docker/03-06-backend-analytics/elasticsearch/Dockerfile) downloads Elasticsearch and installs it on top of the official OpenJDK image.

_Build the database image:_

```
docker image build --tag elasticsearch `
  --file ./docker/03-06-backend-analytics/elasticsearch/Dockerfile .
```


## Running Kibana in Docker

Same story with Kibana, which is the analytics UI that reads from Elasticsearch.

The [Dockerfile](../../docker/03-06-backend-analytics/kibana/Dockerfile) downloads and installs Kibana, and it packages a [startup script](../../docker/03-06-backend-analytics/kibana/kibana.bat) with some default configuration.

_Build the UI image:_

```
docker image build --tag kibana `
  --file ./docker/03-06-backend-analytics/kibana/Dockerfile .
```

## Run the app with analytics

In the [v6 manifest](../../app/03/v6.yml), none of the existing containers get replaced - their configuration hasn't changed. Only the new containers get created.

_Upgrade to v6:_

```
docker-compose -f ./app/03/v6.yml up -d
```

## Check the index handler

The index handler is a different stack from the save handler, but it connects to the message queue in the same way and subscribes to the same events.

_Check the logs and you'll see the connection:_

```
  docker container logs 03_signup-index-handler_1
```

## Refresh your browser

Go back to the sign-up page in your browser. **It's the same set of containers** serving the response, because the app definitions haven't changed.

Add another user and you'll see the data still gets added to SQL Server, but now both message handlers have log entries showing they handled the event message.

## Check the new data is stored

Query the database and print the logs from the message handlers:

```
docker container exec 03_signup-db_1 powershell `
 "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"

docker container logs 03_signup-save-handler_1

docker container logs 03_signup-index-handler_1
```


## Explore the data in Kibana

Kibana is also a web app running in a container, publishing to port `5601` on the Docker host. I'm not proxying Kibana through Traefik - in production it wouldn't be a public component.

_Browse to Kibana at http://localhost:5601_

> The Elasticsearch index is called `prospects`, and you can navigate around the data in Kibana.


## Zero-downtime deployment

The new event-driven architecture lets you add powerful features without updating the original monolith.

There's no regression testing to do for this release, the new analytics functionality won't impact the original app, and power users can build their own Kibana dashboards.
