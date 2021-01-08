# Monitoring Apps - .NET Runtime

Containerized applications give you new opportunities for monitoring. You export metrics from each container, collect them centrally and show your whole application health in a dashboard.

Runtime metrics tell you how hard .NET is working to run your app. Windows Server Core containers store Performance Counters in the usual way and you can [export IIS and .NET Framework Performance Counters](https://github.com/dockersamples/aspnet-monitoring) - but it's better to add metrics to your application source code.

We'll do that using [Prometheus](https://prometheus.io) which is the most popular metrics framework. Prometheus collects metrics from your apps, and your apps expose the statistics which matter to them in an HTTP endpoint.

## Prometheus metrics in console apps

The [prometheus-net](https://github.com/prometheus-net/prometheus-net) NuGet package does that for us. It collects key metrics for .NET Framework and .NET Core apps.

It's already set up in the save handler application - using config in [appsettings.json](../../src/SignUp.MessageHandlers.SaveProspect/appsettings.json) and code in [Program.cs](../../src/SignUp.MessageHandlers.SaveProspect/Program.cs).

The application spec in [save-handler.yml](../../app/04/04-04/save-handler.yml) turns on the metrics server.

_Run the handler and check the metrics endpoint:_

```
cd $env:docker4dotnet

docker-compose -f app/04/web.yml -f app/04/04-04/save-handler.yml up -d signup-save-handler

docker logs 04_signup-save-handler_1
```

> Browse to the metrics endpoint at http://localhost:8090/metrics

## Adding metrics to web apps

The Prometheus client library can be integrated with the ASP.NET Core processing pipeline. It records HTTP request processing times for every controller.

It's configured in the reference data API using the familiar ASP.NET services pattern. The code is all in [Startup.cs](../../src/SignUp.Api.ReferenceData/Startup.cs).

The Compose override [reference-data-api.yml](../../app/04/04-04/reference-data-api.yml) turns metrics on, providing statistics for the ASP.NET runtime.

_Run the API with metrics enabled:_

```
docker-compose -f app/04/web.yml -f app/04/04-04/reference-data-api.yml up -d reference-data-api
```

> Check the metrics at http://localhost:8082/metrics

> Try some API calls with http://localhost:8082/api/roles & http://localhost:8082/roles and refresh the metrics

## Metrics in WebForms apps

The Prometheus client also works for ASP.NET Framework apps - it can be integrated to collect HTTP processing metrics. It will break metrics down to controller level for MVC apps, but for WebForms apps you'll only see the high-level details.

The web app configures the client library in [Global.asax.cs](../../src/SignUp.Web/Global.asax.cs) using an HTTP module. The metrics endpoint is exposed in [Metrics.ashx.cs](../../src/SignUp.Web/Metrics.ashx.cs). It's more work than .NET Core apps, but it gets you a similar set of metrics.

_Run the web app with runtime metrics enabled:_

```
docker-compose -f app/04/web.yml -f app/04/04-04/signup-web.yml up -d signup-web
```

> Browse to the app at http://localhost:8081/app/SignUp and check http://localhost:8081/app/Metrics.ashx

## Key runtime metrics

Runtime metrics tell you how hard your app is working and how well it's handling requests. The .NET Prometheus client gets you some way towards the [SRE Golden Signals](https://www.infoq.com/articles/monitoring-SRE-golden-signals/):

- latency: `http_request_duration_seconds{code="200"}`
- traffic: `http_requests_received_total`
- errors: `http_request_duration_seconds{code="500"}`
- saturation: based on `dotnet_total_memory_bytes` and `process_cpu_seconds_total`

All these components have the same UX for metrics, with a feature flag to turn the metrics server on, a metrics endpoint and an information metric.

The next level of detail comes from application-level metrics which record what the app is actually doing.