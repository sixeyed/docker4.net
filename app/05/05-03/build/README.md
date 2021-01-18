
Builds all the latest versions of the images for lesson 05-02.

```
cd "$env:docker4dotnet/app/05/05-03/build"
```

In Linux container mode:

```
docker-compose -f docker-compose-linux.yml build --pull

docker-compose -f docker-compose-linux.yml push
```



In Windows container mode:

```
docker-compose -f docker-compose-windows.yml build --pull

docker-compose -f docker-compose-windows.yml push

```

