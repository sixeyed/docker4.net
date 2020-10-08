

## config in .net core

appsettings.json

program.cs

ConfigurationBuilderExtensions.cs

```
docker ps

docker logs api

docker rm -f api

docker run -d -p 8082:80 --name api `
  --network section-02 `
  -e Logging:LogLevel:Default=Information `
  reference-data-api
  ```

```
Invoke-RestMethod -Method GET http://localhost:8082/api/roles
```

```
docker logs api
```

## config in net fx

web.config

- log4net.config

```
docker exec signup-web powershell cat /logs/signup.log
```


- dockerfile
- start.ps1

```
cd $env:docker4dotnet

docker image build -t signup-web:02-05 `
  -f ./docker/02-06-platform-integration/signup-web/v5/Dockerfile .

docker rm -f signup-web

docker run -d -p 8081:80 --name signup-web `
  --network section-02 `
  signup-web:02-05
```

http://localhost:8081/app

> add data

```
docker container exec signup-db `
  powershell `
  "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"

docker logs signup-web
```


- web.config
- log4net.config

```
docker image build -t signup-web:02-06 `
  -f ./docker/02-06-platform-integration/signup-web/v6/Dockerfile .

docker run --entrypoint powershell signup-web:02-06 cat /web-app/config/log4net.config

docker run --entrypoint powershell `
-v "${pwd}\app\02-06-platform-integration\config:C:\web-app\config" `
 signup-web:02-06 `
 cat /web-app/config/log4net.config
```


```
    docker rm -f signup-web

docker run -d -p 8081:80 --name signup-web `
-v "${pwd}\app\02-06-platform-integration\config:C:\web-app\config" `
  --network section-02 `
    signup-web:02-06
```



http://localhost:8081/app

> add data

```


docker logs signup-web

docker container exec signup-db `
  powershell `
  "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"
```