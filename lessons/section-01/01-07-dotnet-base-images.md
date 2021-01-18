# .NET Base Images



The base image is the image you use in the `FROM` line in your Dockerfile. It was built with a Dockerfile and it comes packaged with software:

- `nginx` is a Linux image with the web server installed
- `nanoserver` is a Windows image with just the OS installed
- `aspnet` is a Windows image with IIS and ASP.NET installed

Microsoft maintain Docker images for the .NET stack on their own registry, `mcr.microsoft.com`.



## Try a .NET Core image

Switch to Linux container mode in Docker Desktop and download the latest .NET Core 3.1 image.

_Use `docker image pull` to download a runtime image and check it out:_

```
docker image pull mcr.microsoft.com/dotnet/core/runtime:3.1

docker image ls mcr.microsoft.com/dotnet/core/runtime:*
```

> The runtime image is used to run non-web apps.



## Run a .NET Core container on Linux

The image you pulled is a runtime image so it has the `dotnet` command and can run compiled binaries, but you can't use it to compile code.

_Run a container to show the full .NET version built into the image:_

```
docker run mcr.microsoft.com/dotnet/core/runtime:3.1 dotnet --list-runtimes
```

> .NET Core uses semantic versioning. You'll see the latest patch version of the LTS 3.1 branch.



## Try a container with the .NET Core SDK

You can pull images in advance. If you run a container from an image which doesn't exist in your local store then Docker pulls it automatically.

_Run a container from the SDK image:_

```
docker run mcr.microsoft.com/dotnet/core/sdk:3.1 dotnet --list-sdks
```

> The SDK image contains all the build tools.



## And one more runtime image

ASP.NET Core is packaged into a separate base image. It builds on the `runtime` image and adds the web server runtime.

You use the smallest image to give you what you need, so a console app would be based on the `runtime` image and a Web API app would be based on `aspnet`.

_Check the runtimes in the ASP.NET image:_

```
docker run mcr.microsoft.com/dotnet/core/aspnet:3.1 dotnet --list-runtimes
```

> The ASP.NET Core image contains the basic runtime and the ASP.NET runtime.



## Compare the images

All those .NET Core images have different roles. You can use the SDK image to compile apps from source code into container images. You use the runtime image for apps which don't need a web runtime and the ASP.NET image for apps which do.

_List the images to see their sizes:_

```
docker image ls mcr.microsoft.com/dotnet/core/*:3.1
```

> The ASP.NET image is about 20MB bigger than the runtime image, and the SDK image is 3.5x the size of the ASP.NET image. They're taking up storage space on your machine, but we'll be using them all in the course.



## Pull the Windows versions of the .NET Core images

.NET Core is a cross-platform runtime so you can build apps targeted for Windows or Linux. In Docker terms that means you can build Windows **and** Linux images for .NET Core apps using the same source code.

Switch to Windows container mode and you can pull Windows versions of the .NET Core images.

_The Windows images have the same names as the Linux versions:_

```
docker pull mcr.microsoft.com/dotnet/core/runtime:3.1
docker pull mcr.microsoft.com/dotnet/core/aspnet:3.1
docker pull mcr.microsoft.com/dotnet/core/sdk:3.1
```

> Using the same names is confusing now, but it makes things very flexible for your own apps - we'll see that in the next section.



## Check the Windows image sizes

The Windows .NET Core images are based on Nano Server because they don't need the full Windows Server OS to run.

Nano Server images are usually hundreds of megabytes in size, compared to gigabytes for full Windows Server Core images.

_List the images to see their sizes:_

```
docker image ls mcr.microsoft.com/dotnet/core/*:3.1
```

> Each image is about 100MB bigger than its Linux counterpart.



## Choosing between .NET Core and .NET Framework images

You'll build new apps and new components using .NET Core (or .NET 5 when it reaches LTS release) - it's the supported option going forward, and it's lightweight, fast and portable.

Older apps built with the .NET Framework can be packaged to run in Docker containers in the same way, but they're not cross-platform - you can only run them in Windows containers on Windows Servers.

Microsoft provide base images for .NET 4.8 and 3.5, so you can migrate older apps to containers with no code changes.



## Pull the .NET Framework 4.8 images

These are all based on Windows Server Core and they're all much bigger than the .NET Core variants. We'll use them all in the course, and we can save time later by pulling them now.

_The .NET Framework images are published on MCR too:_

```
docker pull mcr.microsoft.com/dotnet/framework/runtime:4.8
docker pull mcr.microsoft.com/dotnet/framework/aspnet:4.8
docker pull mcr.microsoft.com/dotnet/framework/sdk:4.8
```

> They use the same naming standard, but there are only Windows versions of these images.



## Try the tools in the SDK image

The SDK image contains MSBuild together with the Visual Studio build targets and the targeting packs for different .NET Framework versions. You can use this image to compile code in a container.

_Check the versions of the tools:_

```
docker run mcr.microsoft.com/dotnet/framework/sdk:4.8 msbuild -version
```



## And run a default ASP.NET website

The ASP.NET image doesn't contain a default website, so when you run it you'll see an access error. That's because you're meant to use this as the base image and add your own content - like with the [hostname app]().

_Run a web container and check the IIS folder:_

```
docker run -d -p 8080:80 --name aspnet mcr.microsoft.com/dotnet/framework/aspnet:4.8

docker exec aspnet powershell "ls /inetpub"

docker exec aspnet powershell "ls /inetpub/wwroot"
```

> Browse to http://localhost:8080 and you'll see an error because there's no content and directory browsing is turned off.



## The .NET images are listed on Docker Hub

Microsoft serves its images from MCR, but there's no UI for that registry.

You'll use Docker Hub to find the Microsoft images, where you'll see all the tags with the latest versions:

- [.NET Core](https://hub.docker.com/_/microsoft-dotnet) - links to the runtime and SDK repositories
- [.NET Framework](https://hub.docker.com/_/microsoft-dotnet-framework) - links to the runtime and SDK repositories

> Here's where you'll find all the tag aliases too. For the .NET Core runtime `3.1` is an alias for `3.1.8-buster-slim` on Linux, and for `3.1.8-nanoserver-1809` on Windows.



## And the Dockerfiles are on GitHub

All of Microsoft's Docker Hub images are open source, with the Dockerfiles on GitHub.

- [dotnet/dotnet-docker](https://github.com/dotnet/dotnet-docker/tree/master/src) for the .NET Core (and .NET 5) images
- [microsoft/dotnet-framework-docker](https://github.com/microsoft/dotnet-framework-docker/tree/master/src) for .NET Framework 3.5 and 4.8

New images are typically published every month to include the latest OS patch updates, in addition to the release schedule of the runtime.



## That's all the practical work

It's important to have a good understading of the .NET images, because all your apps will ultimately use them as base images.

We'll finish up this section with a recap of everything we've learned.