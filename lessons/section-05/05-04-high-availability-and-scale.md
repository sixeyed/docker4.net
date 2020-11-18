

# resilience

```
vagrant ssh winworker

powershell

docker ps

docker ps -f "name=web" -q

docker exec $(docker ps -f "name=web" -q) powershell "Stop-Service w3svc"

docker ps
```
> replaced with new container (cf. restart with compose)

(browse - no response until healthy)

```
docker ps

docker rm -f $(docker ps -f "name=web" -q) 

docker ps
```


# increase scale - new replicas

> manager

```
docker service update signup_signup-web --replicas 2

docker service ls
```

> winworker

```
docker ps
```

> use app and refresh post - LB causes failure

app\05\05-04\signup-v2.yml

```
docker stack deploy -c app/05/05-04/signup-v2.yml signup

docker service ls
```

> use app and refresh post - sticky in cookie

# configure healthchecks & rollout

app\05\05-04\signup-v3.yml

```
docker stack deploy -c app/05/05-04/signup-v3.yml signup

docker service ps signup_signup-web

docker service ps signup_reference-data-api
```

> new task starts first

# increase capacity - new node


docker stack deploy -c app/05/05-04/analytics-v2.yml analytics

vagrant ssh worker2

ip a

docker swarm join [join-token] --advertise-addr [ip address]

manager:

```
docker node ls

docker node ps worker

docker node ps worker2
```

# force rebalance

```
docker service update signup_reference-data-api --detach --force 

docker service update analytics_index-handler --detach --force

docker service ls

docker node ps worker2
```