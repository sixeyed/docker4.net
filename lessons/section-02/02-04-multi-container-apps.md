v3

dockerfile
config file

```
cd $env:docker4dotnet

docker image build -t signup-web:02-04 `
  -f ./docker/02-04-multi-container-apps/signup-web/Dockerfile .

docker run  --entrypoint powershell dak4dotnet/signup-web:v4 cat /web-app/connectionStrings.config
```


```
docker network ls

docker network create --driver nat section-02

docker network inspect section-02
```

> IP subnet

```
docker run -d --name signup-db `
  --env sa_password=docker4.net! `
  --network section-02 `
  docker4dotnet/sql-server:2017

  docker container inspect signup-db

```

> network is gateway

```
docker run -d -p 8081:80 --name signup-web `
  --network section-02 `
  signup-web:02-04

  docker network inspect section-02

docker exec -it signup-web powershell ping signup-db

docker exec -it web powershell ping signup-db

```

http://localhost:8081/app

> add data

```
docker container exec signup-db `
  powershell `
  "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"
```


