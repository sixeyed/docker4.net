# Monitoring Apps - .NET Runtime

## fx console, save handler

src\SignUp.MessageHandlers.SaveProspect\appsettings.json

src\SignUp.MessageHandlers.SaveProspect\Program.cs

docker-compose -f app/04/web.yml -f app/04/04-04/save-handler.yml up -d signup-save-handler

http://localhost:8090/metrics

## core, ref data

src\SignUp.Api.ReferenceData\SignUp.Api.ReferenceData.csproj

src\SignUp.Api.ReferenceData\Startup.cs

docker-compose -f app/04/web.yml -f app/04/04-04/reference-data-api.yml up -d reference-data-api

http://localhost:8082/metrics

http://localhost:8082/api/roles & refresh metrics

http://localhost:8082/roles & refresh metrics

## web

src\SignUp.Web\Global.asax.cs

src\SignUp.Web\Metrics.ashx.cs

docker-compose -f app/04/web.yml -f app/04/04-04/signup-web.yml up -d signup-web

http://localhost:8081/app/Metrics.ashx


- info metric
- runtime metrics - net runtime, http server, perf counters

- feature flag Metrics:Server:Enabled
