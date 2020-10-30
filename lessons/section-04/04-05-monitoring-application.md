# Monitoring Apps - Custom Metrics

> index handler

src\SignUp.MessageHandlers.IndexProspect\Workers\QueueWorker.cs

docker-compose -f app/04/analytics.yml up -d 

test http://localhost:8081/app/SignUp

docker logs 04_signup-index-handler_1

http://localhost:8091/metrics

docker stop 04_elasticsearch_1

test & check /metrics


> save handler

src\SignUp.MessageHandlers.SaveProspect\Program.cs

docker-compose -f app/04/web.yml -f app/04/04-05/save-handler.yml up -d signup-save-handler

test & check /metrics

docker logs 04_signup-save-handler_1

http://localhost:8090/metrics

> core, ref data

src\SignUp.Api.ReferenceData\Repositories\Spec\RepositoryBase.cs

docker-compose -f app/04/web.yml -f app/04/04-05/reference-data-api.yml up -d reference-data-api

http://localhost:8082/api/roles

http://localhost:8082/metrics

> web

src\SignUp.Web\SignUp.aspx.cs

docker-compose -f app/04/web.yml -f app/04/04-05/signup-web.yml up -d signup-web

test & metrics