

## Pre-reqs

```

cd "$env:docker4dotnet/vagrant/05"

vagrant up
```

## Initialize the Swarm

```
vagrant ssh manager

docker swarm init

docker node ls

docker swarm join-token manager

docker swarm join-token worker

exit
```

> You now have a functional single-node Swarm. The output shows the command you run on other nodes to join the Swarm. 

## Join a Linux worker node

```
vagrant ssh worker

docker swarm join [join-token]

docker info

exit
```

## Join a Windows worker node

```
vagrant ssh winworker

docker swarm join [join-token]

docker info

exit
```

## Check the Swarm


```
vagrant ssh manager

docker node ls

docker node inspect worker

docker node inspect worker-win
```

_Deploy a basic web app:_

```
docker service create --name whoami -p 8080:80 docker4dotnet/whoami

docker service ps whoami

docker node inspect manager -f '{{.Status.Addr}}'

docker node inspect worker -f '{{.Status.Addr}}'
```


> There's a single container running, but you can browse to port `8080` on any node and the traffic gets routed to the container.

_Scale up and more containers will be created - incoming requests get load-balanced between them:_

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

