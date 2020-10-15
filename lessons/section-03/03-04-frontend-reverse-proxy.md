# Managing HTTP Traffic with a Reverse Proxy

A reverse proxy is the central entrypoint to your apps. It receives all incoming requests, fetches the content from another container and sends it back to the consumer.

It's how you run multiple apps in containers and make them all available from the same port.

## The reverse proxy

We'll use [Traefik](http://traefik.io) which integrates nicely with Docker. All requests come to Traefik, and it get content from the homepage, the web app or the API container.

Traefik can do a lot more than that - SSL termination, load-balancing and sticky sessions. There's an [official Traefik image on Docker Hub](https://hub.docker.com/_/traefik) which is built from a [straightforward Dockerfile](https://github.com/containous/traefik-library-image/blob/master/windows/1809/Dockerfile).


## Upgrade to the new homepage

The [v4 manifest](../../app/03/v4.yml) adds services for the homepage and the proxy. The routing rules for the proxy are specified using [container labels](https://github.com/containous/traefik-library-image/blob/master/windows/1809/Dockerfile).

Only the proxy has `ports` specified. It's the public entrypoint to the app, the other containers can access each other, but the outside world can't get to them.

_Update the app to the v4 spec:_

```
docker-compose -f ./app/03/v4.yml up -d
```

> Labels are part of the container spec, so just changes those means containers get replaced.


## See how Traefik is configured

Traefik builds its routing rules from the container labels it sees using the Docker API. Other reverse proxies - like Nginx - need an explicit configuration file.

_Check the Traefik UI at http://localhost:8088_


> _Services_ are the backend containers and _routers_ set up all the routing rules.


## Try the app and the API 

Traefik proxies all traffic from the port `8080`. This setup uses different paths for the components. You can also configure routing base on the HTTP host name.

_Browse to http://localhost:8080 - you get the new homepage, and you can navigate to the original app_

_And browse to the reference data API at http://localhost:8080/api/roles_


## And just to be sure

Check nothing's broken.

Click the _Sign Up!_ button, fill in the form and click _Go!_ to save your details.

_Check the new data is there in the SQL container:_

```
docker container exec 03_signup-db_1 powershell `
  "Invoke-SqlCmd -Query 'SELECT * FROM Prospects' -Database SignUp"
```

## All good

So now we have a reverse proxy which lets us break UI features out of the monolith.

We're running a new homepage with Vue, but we could easily use a CMS for the homepage by running Umbraco in a container - or we could replace the Sign Up form with a separate component using Blazor.

These small units can be independently deployed, scaled and managed. That makes it easy to release front end changes without regression testing the whole monolith.
