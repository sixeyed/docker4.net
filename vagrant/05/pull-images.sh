#!/bin/bash

declare -a images=(
'docker4dotnet/whoami'
'elasticsearch:6.8.12'
'kibana:6.8.12'
'docker4dotnet/index-handler:05-03'
'nats:2.1'
'traefik:v2.3'
'docker4dotnet/reference-data-api:05-03'
)

for i in "${images[@]}"
do
   docker pull "$i"
done