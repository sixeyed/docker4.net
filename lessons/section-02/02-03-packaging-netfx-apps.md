
# Compiling and Packaging .NET Framework Apps

We saw in section 1 that Microsoft publish SDK and runtime images for .NET platforms.

Those images let you use Docker as your build server, compiling from source code without any dependencies - you just need Docker.

This approach is very powerful for hosted build systems like GitHub actions and Azure DevOps. It's powered by [multi-stage Docker builds](https://docs.docker.com/develop/develop-images/multistage-build/).

## Packaging apps with multi-stage Dockerfiles

The approach is simple: 

* use the SDK image as the base, copy in the source code and compile it using the tools in the base image. 

* then use the runtime image as the base for the final app and copy in the compiled app from the previous step.

Here's the [v2 Dockerfile](../../docker/02-03-packaging-netfx-apps/signup-web/v2/Dockerfile) for the Sign Up web app - it uses the existing build script in [build-web.bat](../../src/build-web.bat).

> Multiple `FROM` instructions make this a multi-stage Dockerfile. Only the content from the final stage goes into the app image


## Build the new application image

You work with multi-stage Dockerfiles in the usual way. 

_Switch to the root directory and build the image:_

```
cd $env:docker4dotnet

ls

docker image build -t signup-web:02-02 `
  -f ./docker/02-03-packaging-netfx-apps/signup-web/v2/Dockerfile .
```

> All paths in the Dockerfile are relative to the build context


## Check what's in the final image

The application image has all the content but none of the SDK tools. 

Content from earlier stages only makes it to the final image when it's explicitly copied in.

_Confirm the DLLs are there but not the build tools:_

```
docker container run --rm --entrypoint powershell signup-web:02-02 ls /web-app/bin/SignUp*

docker container run --rm --entrypoint powershell signup-web:02-02 msbuild -version
```

## This is much better than the MSI

Everything is done in Docker, you don't need any build tools on the machine (not even .NET) and everyone will use the same versions of the tools.

But this example isn't perfect - it doesn't make good use of the Docker image layer cache.

_Change a single line in a file and rebuild the image:_

```
Add-Content -Value "" -Path src\SignUp.Web\Web.config

docker image build -t signup-web:02-02 `
  -f ./docker/02-03-packaging-netfx-apps/signup-web/v2/Dockerfile .
```

> Every build starts with a clean SDK environment

## Making use of the build cache

Each Dockerfile instruction generates a layer in the image, and Docker caches the image layers.

During the build Docker generates a hash of the input for each instruction - using the previous state and the new command. If there's a match for the hash in the build cache then that instruction gets skipped.

The [v3 Dockerfile](../../docker/02-03-packaging-netfx-apps/signup-web/v3/Dockerfile) makes use of the cache for the NuGet restore.


## Build the improved version

The build commands are the same for all types of Dockerfile.

_Build the v3 image:_

```
docker image build -t signup-web:02-03 `
  -f ./docker/02-03-packaging-netfx-apps/signup-web/v3/Dockerfile .
```

> The output still contains all the usual build logs


## Compare the images

The MSI image and multi-stage images are functionally pretty much the same.

_Check the image details:_

```
docker image ls signup-web:*
```

> The MSI image is a little bigger, but the v2 and v3 Dockerfiles produce almost identical images


## See the cache in action

The v3 image will only run the NuGet restore if any of the project references have changed. 

You can edit a source file and the app will be compiled again, but the references come from the build cache.

```
Add-Content -Value "" -Path src\SignUp.Web\Web.config

docker image build -t signup-web:02-03 `
  -f ./docker/02-03-packaging-netfx-apps/signup-web/v3/Dockerfile .
```

> Each stage in a multi-stage build has its own cache


## Understanding optimization

Optimizing Dockerfiles improves build time. It can also reduce image size and minimize the application attack surface.

Docker will use the cache as much as it can but when a layer changes then every subsequent instruction gets executed.

You should structure your Dockerfile so the instructions are in order of change frequency, from least frequent to most frequent.


