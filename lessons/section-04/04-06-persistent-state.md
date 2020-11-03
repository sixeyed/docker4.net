# Persisting State

Docker creates the environment for a container, which includes the filesystem. The `C:` drive on Windows containers and the root path on Linux containers are composed of multiple sources - the container image, volume mounts and the container's writeable layer.

Image layers are read-only so they can be shared between images and containers. The writeable layer lets each container modify files without affecting other containers. But the writeable layer has the same lifespan as the container - remove the container and you'll lose all the data it wrote.

For stateful apps you can use volumes and mounts to separate the data lifecycle from the container lifecycle.

## Exploring the container filesystem

The WebForms app writes log entries to a file. We use LogMonitor to relay that file out as container logs but the original data file lives in the container's writeable layer.

_Check the log file in the container:_

```
docker logs 04_signup-web_1

docker exec 04_signup-web_1 powershell ls C:\logs

docker exec 04_signup-web_1 powershell ls C:\web-app
```

> The app files are from the image layers, and the log file is from the writeable layer.

## Specifying volumes in the Docker image

We've already used volume mounts for containers, and there's another way to use them - as Docker Volume objects which you can specify in the Dockerfile.

Volumes live independently of the container they're attached to, so when you remove a container the volume still exists and you can access the files which the container wrote to the volume.

This new [Dockerfile](../../docker\04-06-persistent-state\signup-web\Dockerfile) for the web app uses a volume for the log directory.

_Build and run the new web container and check the configuration:_

```
docker-compose -f app/04/web.yml -f app/04/04-06/signup-web.yml build signup-web

docker-compose -f app/04/web.yml -f app/04/04-06/signup-web.yml up -d signup-web

docker inspect 04_signup-web_1

docker volume ls
```

## Try the app and look at the log files

Browse to http://localhost:8081/app/SignUp and add some new details to confirm the app still works in the same way.

_Check the logs and the physical path of the volume:_

```
docker logs 04_signup-web_1

docker inspect 04_signup-web_1 -f '{{(index .Mounts 0).Source}}'

$source = $(docker inspect 04_signup-web_1 -f '{{(index .Mounts 0).Source}}')

ls $source

cat "$source\SignUp.log"
```

> The volume is a local directory - Docker takes care of mounting it into the container.

## Removing the container doesn't remove the volume

Volumes can be managed independently of containers. If there's a `VOLUME` instruction in the Dockerfile then Docker will create a new volume for each container, but it doesn't get removed when the container is removed.

_Delete the web container and confirm the volume data is still there:_

```
docker rm -f 04_signup-web_1

docker volume ls

ls $source
```

> The log file is still there - this could be a database file or a local cache file.

## Using a named volume for containers

These anonymous volumes are useful for making sure data stays around when containers are removed, but you have to find out which volume you want. Instead you can create a named volume with `docker volume create` or in your Compose spec.

The web container can be configured to write logs to a different location, so we can use an alternative volume:

* [log4net.config](../../app\04\04-06\signup-web-config\log4net.config) writes to the `C:\other-logs` directory
* [LogMonitorConfig.json](../../docker\04-06-persistent-state\signup-web\LogMonitorConfig.json) is configured to read from both log directories
* [signup-web-2.yml](../../app/04/04-06/signup-web-2.yml) uses the new config files and a named volume


## Running the web container with a named volume

Docker Compose has the same desired-state approach for volumes as for containers. The `signup-web-logs` volume doesn't exist so Compose will create it when you bring the application up.

_Replace the web container with the new configuration:_

```
docker-compose -f app/04/web.yml -f app/04/04-06/signup-web-2.yml up -d signup-web

docker logs 04_signup-web_1

docker exec 04_signup-web_1 powershell ls C:\logs

docker exec 04_signup-web_1 powershell ls C:\other-logs
```

> Compose creates the volume with the default driver - the local disk.

## Working with named volumes

Volumes are first-class objects in Docker, you can work with them using the command line.

_Look at the configuration of the new volume:_

```
docker volume ls

docker volume inspect 04_signup-web-logs

docker volume inspect 04_signup-web-logs -f '{{.Mountpoint}}'
```

> The local volume driver uses the disk on the Docker server.


## Named volumes can be reused between containers

This is how you persist state between updates. You can remove the original container and the volume remains. Start a new container with the same volume mount and it will load the existing named volume.

_Replace the web container and check the filesystem in the replacement:_

```
docker rm -f 04_signup-web_1

docker volume ls

docker-compose -f app/04/web.yml -f app/04/04-06/signup-web-2.yml up -d signup-web

cat "$(docker volume inspect 04_signup-web-logs -f '{{.Mountpoint}}')\SignUp.log"

docker logs 04_signup-web_1
```

> The new container sees all the log entries written by the previous container.

## Configuring stateful apps to use volumes

Containers work best for stateless apps because they work well in a dynamic environment - work can be load-balanced between containers easily and containers can come and go without affecting end users. 

You can run stateful apps in containers provided your app can be configured to write data files to a specific location. That location can be a named volume or a bind mount on the server. In the next lesson we'll apply that approach to the SQL Server database.