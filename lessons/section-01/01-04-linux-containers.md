  # Linux Container Basics

Linux containers work in the same way as Windows containers - the Docker commands are the same and the workflow is the same.

Under the hood the container runs Linux on a Linux OS. Docker Desktop takes care of managing the Linux OS for you on Windows. It uses Hyper-V or WSL and takes care of wiring everything up.



## Prerequisites

Open the Docker Dekstop whale icon and click **Switch to Linux containers**. That takes care of the Linux OS for you and sets your Docker command line to use it.

_Run this command to show the OS for the Docker components:_

```
docker version
```

> You should see Linux for the server this time - the client is still Windows.



## Run a task in a container

All the run modes are the same for Linux. You need to use Linux images and Linux commands inside the container.

_Print out the host name from a Linux Alpine container:_

```
docker container run docker4dotnet/alpine hostname
```

> Some commands are the same in Linux and Windows.



## Check for running containers

The Alpine task container still exists, but its in the 'Exited' state.

_List all containers:_

```
docker container ls --all
```

> Just like before, the container ID is the container's hostname. If you had Windows containers running you wouldn't see them in the list - the command line is connecting to the Linux server.



## Run an interactive container

Interactive containers start and connect your console to the container. You can use it like an SSH session, to explore a container.

_Start a Ubuntu container and connect to it:_

```
docker run --interactive --tty --rm `
  docker4dotnet/ubuntu bash
```

> `docker run` is the short form of `docker container run` - both are supported on Linux and Windows.



## Explore Ubuntu

The Ubuntu container is a stripped-down version of the Ubuntu Server installation.

_Explore the container environment:_

```
ls /
ps
apt-get update
curl https://docker4.net
```

> Now run `exit` to leave the Bash session, which stops the container process.



## Run a background SQL container

Detached containers run in the same way. SQL Server is available on Linux too and this will run the Linux version of the database.

_Run SQL Server as a detached container:_

```
docker container run --detach --name sql `
  --env SA_PASSWORD=DockerCon!!! `
  docker4dotnet/mssql:2017
```

> You don't need to specify isolation for Linux containers - they always work with process isolation.



## Exploring SQL Server

Linux containers share a lot more of the host OS. When you look at the processes inside a Linux container you typically only see one or two.

_Check the processes in the SQL container:_

```
docker top sql
```

_And the logs:_

```
docker logs sql
```

> `top` and `logs` can also be used without the `container` prefix



## Running SQL commands

Linux containers don't have PowerShell but the SQL Server image is packaged with the `sqlcmd` utility which you can use to run queries.

_Check what the time is inside the Linux SQL container:_

```
docker exec sql `
  /opt/mssql-tools/bin/sqlcmd -U sa -P 'DockerCon!!!' -Q 'SELECT GETDATE()'
```

> Same with `exec`



## Connect to a background container

The SQL Server container is stil running in the background. The Linux container uses Ubuntu as the base operating system, so Bash is available.

_Connect a Bash session to the container_:

```
docker container exec -it sql bash
```



## Explore the SQL filesystem

The SQL data files live inside the container - you can find the MDF data and LDF log files for the standard databaes.

_Look at the default SQL data directory:_

```
cd /var/opt/mssql
```

```
ls ./data
```



## Processes in the SQL container

The SQL Server processes are running in a separate session from your interactive connection. The `ps` command just shows your session's processes.

_Check the processes running in the container:_

```
ps
```

_But you can add the other sessions to the output:_

```
ps x
```

> There are two `sqlservr` processes running. One is started when the container starts and it spawns the second.



## Linux users in the SQL container

Processes in Linux containers run as standard user accounts - the default in a lot of images is to use the admin user `root`.

_Compare the user accounts for the processes:_

```
ps ux
```

> Everything is running as `root`.



## Those processes are root on the host too

Docker Desktop doesn't give you direct access to the Linux OS which is running the containers.

If you run Docker directly on a Linux machine you can list the processes on the host and you will see the container processes.

`root` in the container is mapped to `root` on the hose.

> This is a security issue which needs to be managed in production.



## Comparing Linux and Windows containers

-  Linux container processes run natively on the host, just like Windows containers

- Container processes usually map an existing user on the host, so breakouts can be dangerous

- The user experience for Linux and Windows containers is the same - it's the same Docker commands



## Disconnect from the container

Exit the interactive Docker session in the SQL Server container:

```
exit
```

> The container is still running - check with `docker ps`



## Clean up all containers

We don't need any of these containers, so you can remove them all

_The `-force` flag works in the same way on Linux:_

```
docker container rm --force `
  $(docker container ls --quiet --all)
```



## That's the basics

Linux and Windows containers have almost identical feature sets and Docker Desktop makes it very easy to switch between them.

But remember than Linux containers run on a Linux OS and Windows containers on a Windows OS. Windows Server machines can run Windows containers but not Linux containers - and Linux server machines can only run Linux containers.

So far we've used images which I've built for this section. Next you'll learn how to build your own images.
