
vagrant ssh manager 


## Shared components - analytics


app\05\05-03\analytics.yml


```
docker network create -d overlay analytics-net

docker network inspect analytics-net
```

```
cd /docker4dotnet

docker stack deploy -c app/05/05-03/analytics.yml analytics

docker stack ps analytics
```

## Shared components - infrastructure

app\05\05-03\infrastructure.yml

```
docker network create -d overlay frontend-net

docker network create -d overlay backend-net

docker stack deploy -c app/05/05-03/infrastructure.yml infrastructure

docker stack ps infrastructure

docker service logs infrastructure_proxy
```

## App configuration

Configs:

```
docker config create signup-web-appsettings app/05/05-03/configs/signup-web-appsettings.json

docker config create signup-web-log4net app/05/05-03/configs/signup-web-log4net.config

docker config create reference-data-config app/05/05-03/configs/reference-data-config.json

docker config ls

docker config inspect --pretty signup-web-log4net

docker config inspect --pretty reference-data-config
```

Secrets:

```
docker secret create signup-db-password app/05/05-03/secrets/signup-db-password

docker secret create signup-web-connectionstrings app/05/05-03/secrets/signup-web-connectionStrings.config

docker secret create save-handler-connectionstrings app/05/05-03/secrets/save-handler-connectionStrings.config

docker secret create reference-data-secret app/05/05-03/secrets/reference-data-secret.json

docker secret ls

docker secret inspect --pretty reference-data-secret
```

## App

app\05\05-03\signup.yml

docker\05-03-deploying-stacks\signup-web\Dockerfile


```
docker network create -d overlay signup-net

docker stack deploy -c app/05/05-03/signup.yml signup

docker stack ps signup

docker stack ps signup  -f "desired-state=running"
```

> Use app at [ip]:8080



```
docker service logs signup_signup-web

docker service logs signup_save-handler

docker service logs signup_index-handler
```