# Centralizing Log Collection

We've prepared our Docker images so that the application logs get surfaced as container logs. Now we have lots of containers running we can take advantage of Docker's pluggable logging framework to collect all those logs and store them in a centralized database.

The EFK stack is a popular choice for Docker and Kubernetes clusters: Elasticsearch is a no-SQL database used to store and index the log entries; Fluentd collects all the container logs and stores them in Elasticsearch; and Kibana is a web front-end to Elasticsearch.

We'll run all those components in containers, to collect the application logs from the .NET containers.

## Configuring Fluentd

Fluentd uses a pipeline approach, where log entries get ... That pipeline is configured in [fluentd-signup.conf](../../app/05/05-05/configs/fluentd-signup.conf).

_Create a config object for Fluentd:_

```
docker config create fluentd-signup app/05/05-05/configs/fluentd-signup.conf
```


app/05/05-05/fluentd.yml

```
docker stack deploy -c app/05/05-05/fluentd-global.yml fluentd

docker stack ps fluentd

docker service logs fluentd_fluentd
```

app/05/05-05/analytics-v3.yml

```
docker stack deploy -c app/05/05-05/analytics-v3.yml analytics
````

> use app, browse to kibana add fluentd index pattern- index-handler logs

- service  name, app name, container name

app/05/05-05/signup-v4.yml

```
docker stack deploy -c app/05/05-05/signup-v4.yml signup

```

docker service ps signup_reference-data-api -f "desired-state=running"

browse to api :8080/api/roles

> refresh Kibana 


edit - add manager ip address
app/05/05-05/signup-v5.yml

```
docker stack deploy -c app/05/05-05/signup-v5.yml signup

```

> add data, refresh kibana - filter on save handler, get event ID & search

could also use daemon.json
