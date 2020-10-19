# Logging Revisited

logmonitor - MSFT tool, multiple channels

## Logging in SQL Server

Dockerfile docker\04-02-logging-revisited\signup-db\Dockerfile

docker\04-02-logging-revisited\signup-db\LogMonitorConfig.json

docker\04-02-logging-revisited\signup-db\start.ps1

> logmonitor
> svcmonitor

```
docker-compose -f app/04/web.yml build signup-db
```

## Logging in ASP.NET

docker\04-02-logging-revisited\signup-web\LogMonitorConfig.json

docker\04-02-logging-revisited\signup-web\Dockerfile

cf docker\02-06-platform-integration\signup-web\v6\start.ps1

```
docker-compose -f app/04/web.yml build signup-web
```

## Run core web components

app\04\web.yml

```
docker-compose -f app/04/web.yml up -d
```

## Test and check logs

TODO

## Logging in Console Apps

```
docker logs - refdatA - save handler
```

netfx - docker\03-05-backend-async-messaging\save-handler\Dockerfile

dotnet - docker\02-05-packaging-dotnet-apps\reference-data-api\Dockerfile

## Application health

Logmonitor as wrapper - failure in app triggers failure , bubbles to \docker
