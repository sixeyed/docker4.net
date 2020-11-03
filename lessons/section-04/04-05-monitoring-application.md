# Monitoring Apps - Custom Metrics

Application-level metrics record details about what your app is doing. You need to write code to collect the metrics you care about, and they get published in the same HTTP endpoint as the runtime metrics.

The Prometheus client library makes it easy to collect metrics and the demo app has metrics collection in all the components, which can be enabled with more feature flags.

## Index handler metrics

The metrics you collect will depend on what your app is doing and what you want to see in your health dashboards. A good starting point is to look at where you're writing logs, and record counts of those events as metrics.

The index handler which writes data to Elasticsearch does that in [QueueWorker.cs](../../src/SignUp.MessageHandlers.IndexProspect/Workers/QueueWorker.cs).

It records metrics about the events it receives, along with a processing status.

## Run the index handler with app metrics

A separate Docker Compose file [analytics.yml](../../app/04/analytics.yml) runs the analytics part of the solution alongside the web components. The index handler is configured to record application metrics.

_Start the analytics stack and check the handler metrics:_

```
cd $env:docker4dotnet

docker-compose -f app/04/analytics.yml up -d 

docker logs 04_signup-index-handler_1
```

> Browse to http://localhost:8091/metrics

## Test the index handler metrics

There are no event-processing metrics yet because the handler hasn't received any. Add some details at http://localhost:8081/app/SignUp and check the outcome.

_Try adding data with and without Elasticsearch running:_

```
docker logs 04_signup-index-handler_1

# refresh http://localhost:8091/metrics

docker stop 04_elasticsearch_1

# add more details at http://localhost:8081/app/SignUp

docker logs 04_signup-index-handler_1

# refresh http://localhost:8091/metrics
```

> You'll see there are counts for events which are `processed` and `failed`.

## Save handler metrics

The .NET Framework message handler can also be configured to record application metrics. It uses the same metric name and labels as the index handler to record event processing.

The code is in the [Program](../../src/SignUp.MessageHandlers.SaveProspect/Program.cs) class and it's pretty much the same as the .NET Core component. It's configured in the Compose file [save-handler.yml](../../app/04/04-05/save-handler.yml).

_Run the handler and check the metrics:_

```
docker-compose -f app/04/web.yml -f app/04/04-05/save-handler.yml up -d signup-save-handler

# add more details at http://localhost:8081/app/SignUp

docker logs 04_signup-save-handler_1
```

> Browse to http://localhost:8090/metrics

## Reference data API metrics

Metrics should record data that you can include in dashboards to see if your app is working as expected. Message handlers record a count of failed messages, and we could raise an alert if that's above 0.

The REST API already records HTTP processing time in the runtime metrics. We can add a lower level of detail in application metrics - in the [RepositoryBase](../../src/SignUp.Api.ReferenceData/Repositories/Spec/RepositoryBase.cs) class it records the number of SQL queries.

_Start the API with app metrics:_

```
docker-compose -f app/04/web.yml -f app/04/04-05/reference-data-api.yml up -d reference-data-api
```

> Browse to http://localhost:8082/api/roles and check metrics at http://localhost:8082/metrics

## Web application metrics

The final component is the WebForms app. In the [SignUp](../../src/SignUp.Web/SignUp.aspx.cs) class it records the number of prospects who sign up and the number of times the web app calls the reference data API.

It uses the same configuration approach, with feature flags in [signup-web.yml](../../app/04/04-05/signup-web.yml) to turn on different levels of metrics.

_Run the web app with metrics enabled:_

```
docker-compose -f app/04/web.yml -f app/04/04-05/signup-web.yml up -d signup-web

# add more details at http://localhost:8081/app/SignUp

docker logs 04_signup-web_1
```

> Check the metrics at http://localhost:8081/app/Metrics.ashx

## Building an application dashboard

We now have all the raw data to give us a detailed overview of how the app is performing. We can see how many prospects are signing up and we can correlate that with event processing, which will show if the handlers are keeping up with the workload.

There are performance metrics in here too, and we can see if the web app is making heavy use of the API or the API is making heavy use of the database. Those can help with tuning exercises, to see if there will be useful improvements with caching. 

We'll see the final dashboard in section 5 and we'll finish up here by looking at managing application data in containers.