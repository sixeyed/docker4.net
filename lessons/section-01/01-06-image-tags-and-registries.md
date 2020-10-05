# Docker Images, Tags and Registries



Images are the portable package that contains your application - your binaries, all the dependencies and the default configuration.

Tags give the image a name and they're also used for versioning.

You share images by pushing them to a registry. [Docker Hub](https://hub.docker.com/) is the most popular public registry. Most enterprises run their own private registry - like [Azure Container Registry](https://docs.microsoft.com/en-us/azure/container-registry/).

You work with all registries in the same way.



## Registry username

You've built some images but you can't push them to a registry yet. To push to Docker Hub your images need to have your username in the image tag so the registry can identify the owner.

_Start by capturing your Docker ID in a variable:_

```
$env:dockerId='<insert-your-docker-id-here>'
```

> Make sure you use your Docker ID, which is the username you use on Docker Hub, not your email address. Mine is `sixeyed`, so I run `$env:dockerId='sixeyed'`



## Image tags

Now you can tag your images. This is like giving them an alias - Docker doesn't copy the image, it just adds a new tag to the existing image.

_Add a new tag for your tweet image which includes your Docker ID:_

```
docker image tag tweet-app "$env:dockerId/tweet-app"
```



## List your images

You can list all your local images with `docker image ls` and you can add filters to the list.

_Show the first tweet image you built and the new tag:_

```
docker image ls tweet-app

docker image ls "$env:dockerId/tweet-app"
```

> They have the same image ID - they're two tags for the same image. You can push image tags with your Docker ID to Docker Hub.



## Login to Docker Hub

You can use any tag for local images - you can use the `microsoft` tag if you want, but you can't push them to Docker Hub unless you have access.

_Log in to Docker Hub with your Docker ID:_

```
docker login --username "$env:dockerId"
```

> You have access to your own user image repositories on Docker Hub, and you can also be granted access to organization repositories.



## Push images to Docker Hub

[Docker Hub](https://hub.docker.com) is the public registry for Docker images.

_Upload your image to Hub:_

```
docker image push $env:dockerId/tweet-app
```

> You'll see the upload progress for each layer in the Docker image.



## Browse to Docker Hub

Open your user page on Docker Hub and you'll see the image is there.

```
start "https://hub.docker.com/r/$env:dockerId/tweet-app/tags"
```

Docker Hub shows some basic information about the image:

- the OS and CPU architecture
- the image size (this is a big image, because it's based on Windows Server)

> This is a public image, so anyone can run containers from it - and the app will work in exactly the same way everywhere.



## How about Linux images?

Each Docker Engine has its own local store of images. You only see the Windows images when you're in Windows container mode and the Linux images in Linux container mode.

_Switch to Linux container mode and look for the tweet app:_

```
docker image ls tweet-app

docker image ls "$env:dockerId/tweet-app"

```

> You only see the Linux version which doesn't have your Docker username.



## Tag the Linux image

Tagging works in the same way for Linux containers. You can add an alias so this image can be pushed to Docker Hub too.

_Add a new tag with your Hub username:_

```
docker image tag tweet-app:linux "$env:dockerId/tweet-app:linux"
```



## Push the Linux image

Image repositories are for remote storage. They store all the image layers as compressed files and the format is the same for all OS and CPU architectures.

_Push the Linux image:_

```
docker push "$env:dockerId/tweet-app:linux"
```



## Check on Docker Hub

We've used the same repository name for both the images we pushed - they have different tags to identify variations. 

_Browse to your Docker Hub repository again:_

```
start "https://hub.docker.com/r/$env:dockerId/tweet-app/tags"
```

> You'll see the Linux version is there too. Repositories are a way of grouping different versions or variants of the same app.

## Using tags for versioning

You've used simple tags so far. You can store many versions of the same app in a single repository by adding a version number to the tag.

_Build a new version of the Linux Tweet app, tagged as `v2`:_

```
cd ./docker/01-05-dockerfiles-and-images/tweet-app-linux-v2

docker build -t "$env:dockerId/tweet-app:v2-linux" .
```

> You can use any versioning scheme you like in the image tag. Semantic versioning is popular, together with an OS identifier - e.g. `nginx:1.18.0-alpine`



## Push a new version of the app

A repository on Docker Hub can store many image variants, all with different tags.

_Push the `v2` tagged image:_

```
docker image push "$env:dockerId/tweet-app:v2-linux"
```

> Refresh the tags page of your Docker Hub repo, you'll see three tags listed.



## What exactly gets uploaded?

The logical size of those images mostly comes from the base image - the Windows version is nearly 2GB each, but the bulk of that is in the Windows Server Core base image.

Those layers are already known by Docker Hub, so they don't get uploaded - only the new parts of the image get pushed.

Docker shares layers between images, so every image that uses Windows Server Core will share the cached layers for that image.



## That's it for the 101

You have a good understanding of the Docker basics now: Dockerfiles, images, containers and registries.

That's really all you need to get started Dockerizing your own applications.

We'll wrap up with a closer look at the base images you'll use for .NET apps.
