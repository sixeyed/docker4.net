- prometheus

app/05/05-06/configs/prometheus.yml

```
docker config create prometheus-config app/05/05-06/configs/prometheus.yml
```


config - grafana

```
docker config create grafana-datasources app/05/05-06/configs/grafana-datasources.yaml

docker config create grafana-providers app/05/05-06/configs/grafana-providers.yaml

docker config create signup-dashboard app/05/05-06/configs/signup-dashboard.json
```

docker stack deploy -c app/05/05-06/metrics.yaml metrics

docker stack ps metrics

ip:9090 - service discovery & targets

```
docker stack deploy -c app/05/05-06/analytics-v4.yml analytics

docker stack ps analytics -f "desired-state=running"
```

ip:9090 - service discovery

/graph
- app info
- process_cpu_seconds_total

```
docker stack deploy -c app/05/05-06/signup-v6.yml signup

docker stack ps signup -f "desired-state=running"
```

ip:9090 - service discovery

- app info
- process_cpu_seconds_total

ip:3000

- dashboard
