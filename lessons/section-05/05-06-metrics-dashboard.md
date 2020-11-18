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

docker stack deploy

localhost:9090

- app info, query metrics

localhost:3000

- dashboard
