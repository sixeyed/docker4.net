


> dockerfile

```
cd $env:docker4dotnet

cd docker/02-02-packaging-pre-built-apps/signup-web/v1

docker image build -t signup-web:02-01 .
```



```
docker run -d -p 8080:80 --name web signup-web:02-01
```

http://localhost:8080/signup

```
docker exec -it web powershell

ls

cd docker4.net\SignUp.Web

cat web.config

cat .\connectionStrings.config

exit
```

```
docker run -d --name SIGNUP-DB-DEV01 `
  --env sa_password=DockerCon!!! `
  docker4dotnet/sql-server:2017

docker exec -it web powershell ping SIGNUP-DB-DEV01
```

http://localhost:8080/signup


Browse wxs

- publish web first then build msi
