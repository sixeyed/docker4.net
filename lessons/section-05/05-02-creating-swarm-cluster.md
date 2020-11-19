# Creating a Docker Swarm Cluster

[Swarm mode]() is the orchestration feature built into Docker. It's a production-grade orchestrator which uses the familiar Docker Compose format to model applications.

We'll create a Swarm cluster for this section and deploy some apps to get a feel for running containers in production. 

If you plan on using a different orchestrator - like Kubernetes or Nomad - the patterns are the same, so what you learn here will still apply.

## Pre-reqs

We'll run multiple virtual machines to create a production-like cluster, so you'll need to install [Vagrant]() if you want to follow along.

The [Vagrantfile]() sets up four machines with Docker installed and ready to be joined into a cluster.

_Use Vagrant to create 3x Linux and 1x Windows VMs:_

```
cd "$env:docker4dotnet/vagrant/05"

vagrant up
```

## Initialize the Swarm

Swarm mode is a feature of the Docker Engine so you don't need to install any additional software.

_Connect to the manager VM and initialize the cluster:_

```
cd "$env:docker4dotnet/vagrant/05"

vagrant status

vagrant ssh manager

docker swarm init
```

> You now have a functional single-node Swarm. The output shows the command you run on other nodes to join the Swarm. 

_You can manage the other nodes from the manager:_

```
docker node ls

docker swarm join-token manager

docker swarm join-token worker

exit
```

## Join a Linux worker node

The manager runs the orchestration components. You add worker nodes to run your application containers. 

_Add a Linux worker node:_

```
vagrant ssh worker

ip a

# docker swarm join [join-token] --advertise-addr [ip address]
```

> Docker can work out the machine's IP address, but you should specify it if you have multiple addresses.

_Check the Docker Engine status:_

```
docker info

exit
```

## Join a Windows worker node

Docker Swarm works in the same way on Windows and Linux. You use the same join command, and Windows nodes can be managers or workers.

_Join the Windows node:_

```
# password :vagrant
vagrant ssh winworker

ipconfig

# docker swarm join [join-token] --advertise-addr [ip address]
```

## Run a simple app

Worker nodes just run your workloads, they're assigned work by the manager. You run `docker` commands on the manager and typically leave the workers alone.

_Check the Swarm setup:_

```
vagrant ssh manager

docker node ls

docker node inspect worker

docker node inspect winworker
```

You run apps in Swarm Mode as **services**, using the same compute abstraction as Compose. A service can run in one or more containers on any nodes.

_Deploy a basic web app:_

```
docker service create --name whoami -p 8080:80 docker4dotnet/whoami

docker service ps whoami

docker node inspect worker -f '{{.Status.Addr}}'

docker node inspect manager -f '{{.Status.Addr}}'
```

> There's a single container running the service, but you can browse to port `8080` on any node and the traffic gets routed to the container.

Scale up and more containers will be created. Publishing a port in Swarm mode uses the ingress network, so incoming requests get load-balanced between containers. 

_Update the service to scale up:_

```
docker service update --replicas 10 whoami

docker service ps whoami

url="$(docker node inspect winworker -f '{{.Status.Addr}}'):8080"

echo $url

curl $url
```

> This is a multi-architecture .NET Core image, so it can run on Windows or Linux. Repeat the curl command to see different OS responses.

_Check the logs and clear up:_

```
docker service logs whoami

docker service rm whoami

docker ps
```

Services are first-class citizens which you administer on the manager node. Swarm mode enables extra features in the Docker CLI, but you can only use them on the manager - typically you'd set up a remote connection.

## Modelling apps for Docker Swarm 

Now we have a cluster running and we've seen how services are created and deployed as containers on the nodes. 

Next we'll see how the Docker Compose format supports Swarm mode. You can model your applications to deploy them to the cluster and take full advantage of the orchestration features.