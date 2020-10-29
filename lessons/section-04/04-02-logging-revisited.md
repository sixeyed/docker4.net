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
docker container rm -f $(docker container ls -aq)

docker-compose -f app/04/web.yml up -d
```

> Not using proxy - local/debug mode

## Test and check logs

_Add some new data at http://localhost:8081/app/SignUp and check it makes it to the databases:_

```
docker container exec 03_signup-db_1 powershell `
 "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"
```

docker logs 04_signup-db_1 - event logs

docker logs 04_signup-web_1 - file logs

docker top 04_signup-web_1 - logmon & svcmon

## Logging in Console Apps

```
docker logs 04_reference-data-api_1

docker top 04_reference-data-api_1
```

netfx - docker\03-05-backend-async-messaging\save-handler\Dockerfile

dotnet - docker\02-05-packaging-dotnet-apps\reference-data-api\Dockerfile

## Application health

Logmonitor as wrapper - failure in app triggers failure , bubbles to \docker

docker ps

docker exec - stop-service w3c

docker ps
