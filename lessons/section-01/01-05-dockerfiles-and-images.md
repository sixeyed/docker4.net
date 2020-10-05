# Dockerfiles and Docker Images



You package your own apps as Docker images using a [Dockerfile](https://docs.docker.com/engine/reference/builder/).

The Dockerfile syntax is straightforward. In this section you'll walk through two Dockerfiles which package websites to run in Windows Docker containers.



## ASP.NET apps in Docker

Have a look at the [Dockerfile for this app](../../docker/01-05-dockerfiles-and-images/hostname-app/Dockerfile). It builds a simple ASP.NET website that displays the host name of the server. There are only two instructions:

- [FROM](https://docs.docker.com/engine/reference/builder/#from) specifes the image to use as the starting point for this image
- [COPY](https://docs.docker.com/engine/reference/builder/#copy) copies a file from the host into the image, at a known location.

The Dockerfile copies a simple `.aspx` file into the content directory for the default IIS website.

> IIS and ASP.NET 4.8 are already installed and configured in the base image.



## Build a simple website image

You package an app by building a Docker image from a Dockerfile. This is a .NET Framework app so it can only run on Windows containers. Switch to Windows container mode in Docker Desktop.

_Change to the app directory and build the Docker image:_

```
cd ./docker/01-05-dockerfiles-and-images/hostname-app

docker image build --tag hostname-app .
```

> The output shows Docker executing each instruction in the Dockerfile, and tagging the final image.



## Run the new app

Now you can run a container from your image to run the app.

_Run a detached container with the HTTP port published:_

```
docker container run --detach --publish 8081:80 `
  --name app hostname-app
```

> Any traffic coming into the server on port 8081 will be managed by Docker and sent to the container.



## Browse to the app

The app in the container is listening on port 80, but you could be using that port on your host.

So we've published port 80 on the container to port 8081 on the host. 

Traffic coming into port 8081 is captured by Docker and sent into the container.

> Browse to the app at http://localhost:8081



## Run multiple instances of the app

Let's see how lightweight the containerized application is.

_Run five containers from the same image:_

```
for ($i=0; $i -lt 5; $i++) {
  & docker run --detach --publish-all --name "app-$i" --isolation=process hostname-app
}
```

> The `publish-all` flag publishes the container port to a random port on the host.



## Check all the containers

You'll have five new containers running, and they'll each be using different host ports to map to port 80.

_List the containers and then check the host port:_

```
docker container ls

docker container port app-1
```



## Check the web apps

You now have multiple instances of the web app running. The Docker image is the same, but each instance will show its own container ID.

_Browse to all the new containers, using this script to find the random host port:_

```
for ($i=0; $i -lt 5; $i++) {
  $address = $(docker port "app-$i" 80).Replace('0.0.0.0', 'localhost')
  start "http://$address"
}
```

> You'll see that each site displays a different hostname, which is the ID of the container running the app.



## See how much compute the containers use

The new containers use process isolation, so you can see the ASP.NET worker processes on your host - that's the `w3wp` process.

_Check the the memory and CPU usage for the apps:_

```
Get-Process -Name w3wp | select Id, Name, WorkingSet, Cpu
```

> The worker processes usually average around 40MB of RAM and <1 second of CPU time.



## Some issues to fix...

This is a simple ASP.NET website running in Docker, with just two lines in a Dockerfile. But there are two issues we need to fix:

- It took a few seconds for the site to load on first use
- We're not getting any IIS logs from the container

> The cold-start issue is because the IIS service doesn't start a worker process until the first HTTP request comes in.



## Logs inside containers

IIS stores request logs in the container filesystem, but Docker is only listening for logs on the standard output from the startup program.

_Check the logs from one of the app containers:_

```
docker container logs app-0
```

> Nothing. The logs are locked inside the container filesystem, Docker doesn't know about them.



## Build and run a more complex web app

The next [Dockerfile](../../docker/01-05-dockerfiles-and-images/tweet-app/Dockerfile) fixes those issues. These are the new instructions it uses:

- [SHELL](https://docs.docker.com/engine/reference/builder/#shell) switches to use PowerShell instead of the Windows command line
- [RUN](https://docs.docker.com/engine/reference/builder/#run) executes some PowerShell to configure IIS to write all log output to a single file
- [ENTRYPOINT](https://docs.docker.com/engine/reference/builder/#entrypoint) configures a [startup script](../../docker/01-05-dockerfiles-and-images/tweet-app/start.ps1) to run when the container starts

> This Dockerfile switches the [escape](https://docs.docker.com/engine/reference/builder/#escape) character to use backticks so file paths can use normal Windows backslashes.



## Build the Tweet app

Build an image from this new Dockerfile.

```
cd ./docker/01-05-dockerfiles-and-images/tweet-app

docker build -t tweet-app .
```

> `docker build` is the short form of `docker image build`



## Run the new app

This is a static HTML site, but you run it in a container in the same way as the last app:

```
docker  run -d -p 8080:80 `
  --name tweet-app tweet-app
```



## Browse to the new app

You can reach the site by browsing to `localhost` (or to your computer externally on port `8080`).

> Browse to the app at http://localhost:8080, there's no startup lag

_Feel free to hit the Tweet button, sign in and share your progress :)_



## And check the IIS logs 

They're being relayed from the log file to the standard output stream.

That's where Docker collects them so now you'll see the access logs.

```
docker container logs tweet-app
```



## How about Linux images?

The Dockerfile syntax is the same and the `docker build` command is the same.

What goes into the Dockerfile is different because you'll use a Linux base image, Linux shell commands and Linux file paths.

[This Dockerfile](../../docker/01-05-dockerfiles-and-images/tweet-app-linux/Dockerfile) is a Linux version of the tweet app. 

> It doesn't need any additional config because the base image already does all we need.



## Build the Linux version

You need to switch to Linux container mode in Docker Desktop. **Your Windows containers are still running** but the Docker CLI is now connected to your Linux OS.

_Build the Linux version of the app:_

```
cd ./docker/01-05-dockerfiles-and-images/tweet-app-linux

docker build -t tweet-app:linux .
```



## Run the Linux version

Your Windows containers are still running so you can't re-use the ports they have allocated.

_Check the containers and run the new tweet app:_

```
docker ps

docker run -d -p 8082:80 tweet-app:linux
```

> Browse to http://localhost:8082 to see the new app. The Windows version is still there at http://localhost:8080/



## Clean up

You can have Windows and Linux containers running at the same time in Docker Desktop. 

When you switch modes you're changing the Docker CLI setup to point to the Docker Engine running on Windows, or on the Linux OS.

_Remove all the containers - first on Linux:_

```
docker container rm --force `
  $(docker container ls --quiet --all)
```

_Now switch to Windows container mode and run the same command:_

```
docker container rm --force `
  $(docker container ls --quiet --all)
```



## Now you have your own Docker images

Removing containers doesn't remove the images.

You've built your own images from Dockerfiles. Right now they only live on the host where you ran the `docker image build` command.

Next you'll see how to share those images by pushing them to Docker Hub, so anyone can run your apps in containers.
