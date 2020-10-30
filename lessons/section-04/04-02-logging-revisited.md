# Logging Revisited

We've used a simple approach to relay logs from a file, using the PowerShell `Get-Content` cmdlet. It's not a great solution - it's only good to read from a single source, and it's not robust. If the file doesn't exist then the script ends, which means your container exits.

Now we'll revisit logging for containers where the application process runs in the background. We'll use the [LogMonitor](https://github.com/microsoft/windows-container-tools/tree/master/LogMonitor) tool from Microsoft, which is built for containers and can relay logs from multiple sources to the standard output stream.

And we'll couple that with the [ServiceMonitor](https://github.com/microsoft/IIS.ServiceMonitor) tool, which Microsoft also provide for containerized applications. It monitors a background Windows Service and bubbles up failures, so if the Windows Service exits then the container exits.

## Logging in SQL Server

Here's a new [Dockerfile](../../docker/04-02-logging-revisited/signup-db/Dockerfile) for a custom SQL Server container image.

It does a few new things:

* downloads the LogMonitor and ServiceMonitor tools
* overrides the default SA password from the base image
* replaces the startup script from the base image

## Relaying Event Log entries

LogMonitor uses a JSON configuration file to specify the log sources. 

[LogMonitorConfig.json](../../docker/04-02-logging-revisited/signup-db/LogMonitorConfig.json) sets it to read the application Event Log which is where SQL Server write entries.

The [start.ps1](../../docker/04-02-logging-revisited/signup-db/start.ps1) ends by running ServiceMonitor to monitor the SQL Server Windows Service, and LogMonitor to relay the log entries.

> LogMonitor wraps an executing process, so Docker sees the logs relayed by LogMonitor, but if the inner process exits then so does the container.

## Build the new SQL Server image

There are multiple Docker Compose files for this section, breaking the app into different sets of components.

[web.yml](../../app/04/web.yml) defines just the core application components, without the reverse proxy or the analytics features. 

You can use this to run a minimal version of the app, and it includes build details for the database and web images.

_Build the updated database image:_

```
cd $env:docker4dotnet

docker-compose -f app/04/web.yml build signup-db
```

## Logging in ASP.NET Framework web apps

We'll add the same LogMonitor + ServiceMonitor pattern for the ASP.NET WebForms app:

* [LogMonitorConfig.json](../../docker/04-02-logging-revisited/signup-web/LogMonitorConfig.json) is set up to relay logs from the log file

* the [Dockerfile](../../docker/04-02-logging-revisited/signup-web/Dockerfile) adds new stages to download the utilities and sets them as the entrypoint

_Build the new web image:_

```
docker-compose -f app/04/web.yml build signup-web
```

## Run core web components

The Docker Compose file just defines the web app, database, message queue and save handler.

We'll run them all in Windows container mode in this section.

_Clear down any existing containers and run the web components:_

```
docker container rm -f $(docker container ls -aq)

docker-compose -f app/04/web.yml up -d

docker ps
```

> This setup publishes ports for the external components, removing the proxy.

## Try out the app

The app works in the same way, but now the log relay is more reliable for the background apps.

_Sign up at http://localhost:8081/app/SignUp and check the data:_

```
docker container exec 04_signup-db_1 powershell `
 "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"
```

> The cold-start issue is back for the web app, but we'll fix that in the next lesson.

## Check the logs

All the components have a consistent management interface for logging. 

The SQL Server logs are relayed from the Windows Event Log, and the web logs from a file - LogMonitor also supports Event Tracing for Windows (ETW).

_Print the log entries:_

```
docker logs 04_signup-db_1

docker logs 04_signup-web_1
```

## And the running processes

The web and database containers are running multiple processes. ServiceMonitor is watching the background Windows Service and LogMonitor is watching ServiceMonitor.

_Check the processes:_

```
docker top 04_signup-db_1

docker top 04_signup-web_1
```

> These are tiny utilities but they're robust enough for production use.

## Compare logging in the console components

The .NET Core components run as console apps and write directly to the standard output stream. 

They don't need either of the monitoring utilities, because the container process *is* the application process, and it produces logs where Docker sees them.

_Check the logs and processes in the reference data API:_

```
docker logs 04_reference-data-api_1

docker top 04_reference-data-api_1
```

> These containers are more lightweight but the .NET Fx containers have the same UX.

## Application failures stop the container

This is important for self-healing apps - if a component fails then the container should exit. 

In a production environment the container runtime will take action to restart or replace the container.

_Check that the monitor utilities work as expected:_

```
docker ps

docker exec 04_signup-web_1 powershell "Stop-Service w3svc"

docker ps
```

The container is still there, and you can restart it to bring the app online:

```
docker ps -a

docker start 04_signup-web_1
```

> Check the app again at http://localhost:8081/app/SignUp

## Operational consistency

Now our .NET apps all behave in the same way, where logs are available from the container and an application failure causes a container exit.

The next step is to make sure when the containers are running that the app really is working correctly.