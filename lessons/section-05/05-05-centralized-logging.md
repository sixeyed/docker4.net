
app/05/05-05/configs/fluentd-signup.conf

```
docker config create fluentd-signup app/05/05-05/configs/fluentd-signup.conf
```

app/05/05-05/fluentd.yml

```
docker stack deploy -c app/05/05-05/fluentd-global.yml fluentd

docker stack ps fluentd

docker service logs fluentd_fluentd
```

app\05\05-05\signup-v4.yml

```
docker stack deploy -c app/05/05-05/signup-v4.yml signup

```