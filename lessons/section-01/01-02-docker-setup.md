# Setting up your environment

You can follow along with everything in this section using Docker Desktop, which works on Windows 10 and lets you run Linux and Windows containers.

Older versions of Windows can't run Windows containers, so if you don't have Windows 10 available you can run it in a Virtual Machine or use a cloud VM.

## Installing Docker Desktop

Browse to the Desktop homepage:

- https://www.docker.com/products/docker-desktop

Click to download the _Windows (Stable)_ version (the Edge version is fine too but it has beta features that we don't need).

Double-click the .exe to install Docker and follow the screens.

When you're done you can launch Docker Desktop and you'll see Docker's whale icon in your toolbar. 

## Verifying the setup

Docker Desktop can run Linux and Windows containers. We'll start with Windows, so click the whale icon and select _Switch to Windows containers_.

Now open up a PowerShell session and run this command:

```
docker version
```

You should see output telling you the versions for the Docker _client_ (which is the command line), and the Docker _server_ (which is the background service that manages containers).

## Downloading the source materials

The final step is to download the materials for the course which are stored on GitHub.

If you have a Git client you can just clone the repository:

```
git clone https://github.com/sixeyed/docker4.net
```

If not then you can browse to https://github.com/sixeyed/docker4.net, click the _Code_ button and click _Download ZIP_ - then expand the ZIP file.

## Configuring environment variables

There are lots of exercises in the course which you can follow along with. Some need you to be in a specific folder in the source code. It'll make things easier if you save the directory path for your `docker4.net` folder in an environment variable.

You can do this in PowerShell:

```
# use your own path, e.g. mine is 'C:\scm\github\sixeyed\docker4.net'
$env:docker4dotnet='<path-to-the-folder>'
```

Then any time you need to get back to the root folder to follow an exercise, just run:

```
cd $env:docker4dotnet
```

