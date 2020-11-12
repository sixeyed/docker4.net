# increase scale - new replicas

docker service update web

docker service update save-handler

docker service update reference-data

# increase capacity - new node

vagrant up linux2

vagrant ssh linux2

docker swarm join

# force rebalance

docker service update reference-data --force (rebalance)

# deploy global proxy

docker stack deploy proxy (global)
