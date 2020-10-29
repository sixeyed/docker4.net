# Monitoring Apps - .NET Runtime

> console, save handler

src\SignUp.MessageHandlers.SaveProspect\appsettings.json

src\SignUp.MessageHandlers.SaveProspect\Program.cs

docker-compose up -d ... save-handler

http://localhost:80xx/metrics

> core, ref data

src\SignUp.Api.ReferenceData\SignUp.Api.ReferenceData.csproj

src\SignUp.Api.ReferenceData\Startup.cs

docker-compose up -d ... reference-data-api

http://localhost:80xx/metrics

> web

TODO - metrics exporter; update prom client; build & test

- info metric
- runtime metrics - net runtime, http server, perf counters

- feature flag Metrics:Server:Enabled
