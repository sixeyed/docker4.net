# High-availability and Scale

Orchestrators manage your application containers for you. The Swarm manager monitors all the nodes and their containers. It can take corrective action if there are problems, and when you run apps at scale the cluster takes care of load balancing between containers.

The work we did in section 4 to prepare apps for production all comes into play here - we can verify that the Swarm keeps the components running if containers fail. But we also need to model for high-availability to make sure there's no loss of service while the Swarm is fixing problems.

## Self-healing applications

The Swarm will keep your services running at the desired scale. If a container exits it will be replaced. 

In section 4 we used `ServiceMonitor` to make sure the web container exited if the IIS Windows Service stopped.


_Check that if the IIS process stops, the container is replaced:_

```
vagrant ssh winworker

powershell

docker ps

docker ps -f "name=web" -q

docker exec $(docker ps -f "name=web" -q) powershell "Stop-Service w3svc"

docker ps
```

> The Swarm manager creates a new container to replace the exited one - unlike with the `restart` option in Compose which restarts the same container.

There's a single WebForms container and it has a healthcheck in the image. When the replacement container starts running, it won't receive any traffic until it reports as healthy. Browse to the app in the meantime and you'll get an error.

It's the same scenario if the container gets removed or the node goes offline - if there's available capacity in the cluster then a replacement container gets scheduled.

_Remove the new web container:_

```
docker ps -f "name=web" -q

docker rm -f $(docker ps -f "name=web" -q) 

docker ps

exit
```

> Try the web app again and you'll see an error until the new container's healthcheck passes.

## Adding containers provides high-availability

You can run multiple containers for a service to increase availability. The Swarm load-balances incoming requests across all the healthy containers. If a container fails then the others share the load until the replacement comes online.

_Scale up the WebForms service, adding another replica:_

```
vagrant ssh manager

docker service update signup_signup-web --replicas 2

docker service ls

docker service ps signup_signup-web 
```

> When both containers are running, try the app again. The SignUp page renders correctly, but if you try to add some data you'll get an error.

The WebForms app uses an anti-forgery token as a security measure. At scale the POST request can be sent to a different container than the one in the GET request, which causes the failure. 

This app isn't really stateless - it needs all requests from a user session to be handled by the same container. 

## Use sticky sessions with Traefik

We can do that using the sticky session feature in Traefik. The reverse proxy will add a cookie to the first response for a session, and then make sure subsequent requests get routed to the same container.

Using sticky sessions just needs a change to the Traefik labels - that's in [05-04/signup-v2.yml](../../app/05/05-04/signup-v2.yml).

_Deploy the update to use sticky sessions:_

```
docker stack deploy -c app/05/05-04/signup-v2.yml signup

docker service ls
```

> Now try the app, refresh a few times and submit new data. It all works as before (check the cookies in your browser's network tab). 

Sticky sessions are often needed for older apps migrating to containers, and it's easy to configure in the reverse proxy (Nginx and other proxies have similar features).


## Configure healthchecks and the update process

There's lots more to consider when you're running at scale. [05-04/signup-v3.yml](../../app/05/05-04/signup-v3.yml) adds some more production configuration:

* for healthchecks, overriding the frequency and startup period defined in the Docker image
* for rolling updates, setting the order and failure action

_Deploy the changes and see how they're rolled out:_

```
docker stack deploy -c app/05/05-04/signup-v3.yml signup

docker service ps signup_signup-web

docker service ps signup_reference-data-api
```

> New tasks start first, which means an update doesn't reduce capacity.

## You can scale the cluster to increase compute

Adding a new node is as simple as provisioning a machine with Docker installed and joining the Swarm. New nodes add their CPU and memory capacity to the Swarm, but they don't take work from other nodes when they join.

The updated spec for the analytics stack in [05-04/analytics-v2.yml](../../app/05/05-04/analytics-v2.yml) adds more replicas of the index handler. It's a Linux component so the new containers will run on the manager and worker nodes.

_Deploy the updated stack:_

```
docker stack deploy -c app/05/05-04/analytics-v2.yml analytics

docker node ps manager -f "desired-state=running"

docker node ps worker -f "desired-state=running"
```

> Index handler containers are running on both nodes.

We have one more Linux VM in the Vagrant setup which we can join to the Swarm.

_Connect to the VM and join the Swarm:_

```
vagrant ssh worker2

ip a

docker swarm join [join-token] --advertise-addr [ip address]

exit
```

_Back on the manager node, check the nodes and their workloads:_

```
docker node ls

docker node ps worker -f "desired-state=running"

docker node ps worker2 -f "desired-state=running"
```

> The original worker node is running the same containers and the new node isn't running anything.

## Services don't rebalance automatically

Swarm doesn't reschedule work when a new node joins, because that would mean removing healthy containers - potentially that will damage your apps. But you can manually force the service to be rebalanced with an update, which causes the service to be rescheduled.

_Update the Linux app services and check the services get rebalanced:_

```
docker service update signup_reference-data-api --detach --force 

docker service update analytics_index-handler --detach --force

docker service ls

docker node ps worker2
```

> Now the new node is running containers.

## Preparing for Day 2 operations

Now the application is running well and we've seen how to model high availability, and how the Swarm takes care of load balancing the incoming workload.

The rest of the section will focus on ongoing maintenance. The sample application is now a distributed system running across 15 containers on 4 servers. It's getting hard to manage with the command line, so next we'll see how to set up centralized logging to collect, store and visualize all the container logs.