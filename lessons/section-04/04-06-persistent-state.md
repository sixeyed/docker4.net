# Persisting State

## web app - anonymous volume for logs

```
docker logs 04_signup-web_1

docker exec 04_signup-web_1 powershell ls C:\logs
```


docker\04-06-persistent-state\signup-web\Dockerfile

```
docker-compose -f app/04/web.yml -f app/04/04-06/signup-web.yml build signup-web

docker-compose -f app/04/web.yml -f app/04/04-06/signup-web.yml up -d signup-web

docker logs 04_signup-web_1

docker inspect 04_signup-web_1

docker volume ls
```


```
docker inspect 04_signup-web_1 -f '{{(index .Mounts 0).Source}}'

$source = $(docker inspect 04_signup-web_1 -f '{{(index .Mounts 0).Source}}')

ls $source

cat "$source\SignUp.log"
```

```
docker rm -f 04_signup-web_1

docker volume ls

ls $source
```


- see logs written from container
- lost when container removed

## web app - named volume for logs

docker\04-06-persistent-state\signup-web\LogMonitorConfig.json

app\04\04-06\signup-web-config\log4net.config

app/04/04-06/signup-web-2.yml

```
docker-compose -f app/04/web.yml -f app/04/04-06/signup-web-2.yml up -d signup-web

docker logs 04_signup-web_1

docker exec 04_signup-web_1 powershell ls C:\logs

docker exec 04_signup-web_1 powershell ls C:\other-logs
```

```
docker volume ls

docker volume inspect 04_signup-web-logs

docker volume inspect 04_signup-web-logs -f '{{.Mountpoint}}'
```

```
docker rm -f 04_signup-web_1

docker volume ls

docker-compose -f app/04/web.yml -f app/04/04-06/signup-web-2.yml up -d signup-web

cat "$(docker volume inspect 04_signup-web-logs -f '{{.Mountpoint}}')\SignUp.log"

docker logs 04_signup-web_1
```

- see logs written from container
- retained with replacement

## sql - bind mount for data folder

- custom startup script
- check for db files before loading

docker\04-06-persistent-state\signup-db\Dockerfile

docker\04-06-persistent-state\signup-db\start.ps1


```
docker-compose -f app/04/web.yml -f app/04/04-06/signup-db.yml build signup-db

docker-compose -f app/04/web.yml -f app/04/04-06/signup-db.yml up -d signup-db

docker container logs 04_signup-db_1
```

```
docker container restart 04_signup-web_1 04_signup-save-handler_1
```

test

```
docker container exec 04_signup-db_1 powershell `
  "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"
```

- with bind mount

app/04/04-06/signup-db-2.yml

mkdir -p C:\databases 

docker-compose -f app/04/web.yml -f app/04/04-06/signup-db-2.yml up -d signup-db

ls C:/databases

docker container restart 04_signup-web_1 04_signup-save-handler_1

test

 app/04/04-06/signup-db-3.yml

docker inspect 04_signup-db_1 -f '{{.Id}}'

docker-compose -f app/04/web.yml -f app/04/04-06/signup-db-3.yml up -d signup-db

docker logs 04_signup-db_1 

docker inspect 04_signup-db_1 -f '{{.Id}}'

docker container exec 04_signup-db_1 powershell `
  "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"
  