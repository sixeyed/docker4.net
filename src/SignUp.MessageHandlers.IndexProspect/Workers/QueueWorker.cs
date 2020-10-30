using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using NATS.Client;
using Prometheus;
using SignUp.Core;
using SignUp.MessageHandlers.IndexProspect.Indexer;
using SignUp.Messaging;
using SignUp.Messaging.Messages.Events;

namespace SignUp.MessageHandlers.IndexProspect.Workers
{
    public class QueueWorker
    {
        private static ManualResetEvent _ResetEvent = new ManualResetEvent(false);

        private const string QUEUE_GROUP = "index-handler";        
        private const string HANDLER_NAME ="IndexProspect";
        private static Counter _EventCounter ;

        private readonly MessageQueue _queue;
        private readonly IConfiguration _config;
        private readonly Indexer.Index _index;

        public QueueWorker(MessageQueue queue, IConfiguration config, Indexer.Index index)
        {
            _queue = queue;
            _config = config;
            _index = index;
        }

        public void Start()
        {
            if (_config.GetValue<bool>("Metrics:Server:Enabled"))
            {
                StartMetricServer();
                if (Config.Current.GetValue<bool>("Metrics:Application:Enabled"))
                {
                    _EventCounter = Metrics.CreateCounter("message_handler_events", "Event count", "handler", "status");
                }
            }            

            Console.WriteLine($"Connecting to message queue url: {Config.Current["MessageQueue:Url"]}");
            using (var connection = _queue.CreateConnection())
            {
                var subscription = connection.SubscribeAsync(ProspectSignedUpEvent.MessageSubject, QUEUE_GROUP);
                subscription.MessageHandler += IndexProspect;
                subscription.Start();
                Console.WriteLine($"Listening on subject: {ProspectSignedUpEvent.MessageSubject}, queue: {QUEUE_GROUP}");

                _ResetEvent.WaitOne();
                connection.Close();
            }
        }

        private void IndexProspect(object sender, MsgHandlerEventArgs e)
        {
            if (_EventCounter != null)
            {
                _EventCounter.Labels(HANDLER_NAME, "processed").Inc();
            }

            Console.WriteLine($"Received message, subject: {e.Message.Subject}");
            var eventMessage = MessageHelper.FromData<ProspectSignedUpEvent>(e.Message.Data);
            Console.WriteLine($"Indexing prospect, signed up at: {eventMessage.SignedUpAt}; event ID: {eventMessage.CorrelationId}");

            var prospect = new Documents.Prospect
            {
                CompanyName = eventMessage.Prospect.CompanyName,
                CountryName = eventMessage.Prospect.Country.CountryName,
                EmailAddress = eventMessage.Prospect.EmailAddress,
                FullName = $"{eventMessage.Prospect.FirstName} {eventMessage.Prospect.LastName}",
                RoleName = eventMessage.Prospect.Role.RoleName,
                SignUpDate = eventMessage.SignedUpAt
            };

            try
            {
                _index.CreateDocument(prospect);
                Console.WriteLine($"Prospect indexed; event ID: {eventMessage.CorrelationId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Index prospect FAILED, email address: {prospect.EmailAddress}, ex: {ex}");
                if (_EventCounter != null)
                {
                    _EventCounter.Labels(HANDLER_NAME, "failed").Inc();
                }
            }
        }
        
        private void StartMetricServer()
        {
            var metricsPort = Config.Current.GetValue<int>("Metrics:Server:Port");
            var server = new MetricServer(metricsPort);
            server.Start();
            Console.WriteLine($"Metrics server listening on port: {metricsPort}");
        }
    }
}
