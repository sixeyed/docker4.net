# Persistent SQL Databases

Database containers are great for non-production environments where you don't need high-availability and resilient storage. You get all the benefits of fast and lightweight containers and your Docker image can include the database schema.

Isolating the database files from the container with volume mounts means you can update the container with a new version of the schema or a SQL Server upgrade, and attach all the existing data.

## Configuring SQL Server for data mounts

SQL Server uses a default file path for database files. To use volume mounts for data you need to create the database with a specific file location. 

Then your startup script in the container needs to do a couple of things:

* check the data file path to see if any files are already there
* if so then create the database and attach those files
* if not then create a new database using that file path

The logic isn't too complex - it's in the new [start.ps1](../../docker/04-07-persistent-databases/signup-db/start.ps1), which overwrites the script from the base image in the [Dockerfile](../../docker/04-07-persistent-databases/signup-db/Dockerfile).

> This app uses Entity Framework to deploy the schema, but the database container could do that with a Dacpac or a tool like Flyway.

## Update the database container

The current database container is running without any volumes. When it gets replaced all the existing data will be lost, because it's only stored in the container's writeable layer.

_Build the new database image and replace the container:_

```
docker-compose -f app/04/web.yml -f app/04/04-07/signup-db.yml build signup-db

docker-compose -f app/04/web.yml -f app/04/04-07/signup-db.yml up -d signup-db

docker container logs 04_signup-db_1 -f

# Ctrl-C to exit the log follow
```

> The new container creates a new, empty database.

## Confirm the data file location

The web application is broken right now because the database schema is empty. Restarting the web container causes it to deploy the schema to the new database.

_Restart the web and handler containers and check the data storage:_

```
docker container restart 04_signup-web_1 04_signup-save-handler_1

# add some details at http://localhost:8081/app/SignUp

docker container exec 04_signup-db_1 powershell `
  "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"

docker container exec 04_signup-db_1 powershell ls C:/data
```

> The data and log files are in the expected location, but this is still in the container's writeable layer.

## Run a persistent database with a bind mount

The Compose override [signup-db-2.yml](../../app/04/04-07/signup-db-2.yml) uses a bind mount in the volume spec to store the database files on the local disk.

You can use a named volume instead, but the bind mount lets you specify the path on the host.

_Replace the database container with the volume mount spec:_

```
mkdir -p C:/databases 

docker-compose -f app/04/web.yml -f app/04/04-07/signup-db-2.yml up -d signup-db

docker container exec 04_signup-db_1 powershell ls C:/data

ls C:/databases
```

> The data and log files are mounted into the container filesystem from the host.

## Check the database is still working

Volume mounts can use different sources - it's the `C:` drive on the host here but it could be a RAID array or a network share. 

Different filesystems have different feature sets and performance characteristics so you need to test your apps.

_Restart the app containers and test the save workflow:_

```
docker container restart 04_signup-web_1 04_signup-save-handler_1

# add more details at http://localhost:8081/app/SignUp

docker container exec 04_signup-db_1 powershell `
  "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"
```

> Only the new row is there, but now the data is stored outside of the container.

## Replace the database container 

You'll replace containers any time the image changes with an app update, or the specification of the container environment changes.

[signup-db-3.yml](../../app/04/04-07/signup-db-3.yml) publishes the SQL Server port which is an environment change, so it needs a container replacement. The spec uses the same volume mount, so the new database will attach the existing data files.

_Replace the container and check the data is still there:_

```
docker inspect 04_signup-db_1 -f '{{.Id}}'

docker-compose -f app/04/web.yml -f app/04/04-07/signup-db-3.yml up -d signup-db

docker logs 04_signup-db_1 -f

# Ctrl-C when the database is attached

docker inspect 04_signup-db_1 -f '{{.Id}}'

docker container exec 04_signup-db_1 powershell `
  "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"
```

> The new container is using the existing data files, bind mounted from the host.

## Database containers - for real?

This pattern for database containers works well across the team. Developers can run containers without a mount and get a clean database every time they need one. In test environments you can use the same container image but with a mount to preserve user data and test schema updates.

In production you'll run a managed SQL service or an external database server, and just configure the app containers to use it. But if you package your schema in the Docker image you can use a container to deploy schema updates to the production database.

