# Centralizing Log Collection

We've prepared our Docker images so that the application logs get surfaced as container logs. Now we have lots of containers running we can take advantage of Docker's pluggable logging framework to collect all those logs and store them in a centralized database.

The EFK stack is a popular choice for Docker and Kubernetes clusters: [Elasticsearch]() is a no-SQL database used to store and index the log entries; [Fluentd]() collects all the container logs and stores them in Elasticsearch; and [Kibana]() is a web front-end to Elasticsearch.

We'll run all those components in containers, to centralize the application logs from the .NET containers.

## Configuring Fluentd

Fluentd uses a pipeline approach, where log entries get processed through stages - like input, filtering and output. 

Our pipeline is configured in [fluentd-signup.conf](../../app/05/05-05/configs/fluentd-signup.conf), which will store all the Sign Up application logs to Elasticsearch.

_Create a config object for Fluentd:_

```
docker config create fluentd-signup app/05/05-05/configs/fluentd-signup.conf
```

Fluentd can run in containers on Linux or Windows so we can run a container on every node in the cluster. The Fluentd container will collect the logs from the other containers running on the node.

> Right now there are some issues forwarding logs from Fluentd to Elasticsearch on Windows, so we'll restrict the Fluentd containers to run on Linux nodes.

This still gives us what we want. To start with we'll deploy Fluentd with the spec in [fluentd.yml](../../app/05/05-05/fluentd.yml). That runs a global service on the Linux nodes, publishing the Fluentd ports directly to the hosts. 

_Deploy Fluentd:_

```
docker stack deploy -c app/05/05-05/fluentd.yml fluentd

docker stack ps fluentd

docker service logs fluentd_fluentd
```

## Configure app containers to use Fluentd

Docker supports Fluentd as a logging plugin, in standalone Engine mode and in Swarm mode. You can set the logging driver to use when you run a container with the `--log-driver` option, or in the Compose specification.

In [analytics-v3.yml](../../app/05/05-05/analytics-v3.yml) the index handler is configured to use Fluentd. The Docker Engine running those containers will send the logs to Fluentd using the default address `localhost:24224`.

The specification includes a tag which adds metadata for Fluentd to attach to log entries, so we can identify the application and component that created the logs.

_Deploy the updated index handler:_

```
docker stack deploy -c app/05/05-05/analytics-v3.yml analytics
```

> Browse to the app at port `8080` and add some data.

Now you can open Kibana at port `5601` to see the logs. We're using the same Elasticsearch and Kibana instances for logs and business data.

Add a new index pattern for `fluentd*` and in the Discover tab you'll see the container logs, together with the metadata.

## Capture logs from Linux containers

We'll configure the other components for logging too. In [signup-v4.yml](../../app/05/05-05/signup-v4.yml) the reference data API is set to use Fluentd.

These are Linux containers, so Docker will forward logs to the local Fluentd container for each node.

_Update the API:_

```
docker stack deploy -c app/05/05-05/signup-v4.yml signup

docker service ps signup_reference-data-api -f "desired-state=running"
```

> When the new containers are running, browse to any node at `:8080/api/roles` and refresh a few times. Reload the data in Kibana and you'll see the new log entries.

## Capture logs from Windows containers

We're not running Fluentd containers on the Windows node, so Docker can't send logs to the `localhost` address. Instead we'll configure the Windows containers to send logs to the manager node.

> Edit the spec in [signup-v5.yml](../../app/05/05-05/signup-v5.yml) and replace `[MANAGER_IP]` with the actual IP address of the manager node.

_Deploy the updates to the Windows components:_

```
docker stack deploy -c app/05/05-05/signup-v5.yml signup

docker service ps signup_signup-web -f "desired-state=running"
```

When the new web containers are running, refresh the site and add some data. Back in Kibana, refresh and you'll see logs from the WebForms app and the SQL Server save handler.

## Specifying a global log driver

You can set the default logging option at the Docker Engine level, which you'll want for all your container logs to be processed by Fluentd.

The Docker Engine is configured with a JSON file called `daemon.json`. The logging snippet in [daemon.json](../../app/05/05-05/node/daemon.json) configures all container logs to be collected and sent to Fluentd.

You can specify the tag option at the service level in the Compose spec, to customize the metadata for the logs.

## Comparing logging and monitoring

We now have a centralized log store with a front-end we can use for searching and visualizing logs. 

Logs are where you'll look to debug problems when they happen or afterwards - but we're also collecting metrics in our apps and that can help us find problems before they become serious.

In the last lesson for the module we'll take all the raw metrics we put together in section 4 and build them into a really useful monitoring dashboard.