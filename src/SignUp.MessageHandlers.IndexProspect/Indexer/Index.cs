using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Nest;
using SignUp.MessageHandlers.IndexProspect.Documents;
using System;

namespace SignUp.MessageHandlers.IndexProspect.Indexer
{
    public class Index
    {
        private readonly IConfiguration _config;

        public Index(IConfiguration config)
        {
            _config = config;
            EnsureIndex();
        }

        private void EnsureIndex()
        {            
            Console.WriteLine($"Initializing Elasticsearch. url: {_config["Elasticsearch:Url"]}");
            var client = GetClient();
        }

        public void CreateDocument(Prospect prospect)
        {
            GetClient().IndexDocument(prospect);
        }

        private ElasticClient GetClient()
        {
            var uri = new Uri(_config["Elasticsearch:Url"]);
            var settings = new ConnectionSettings(uri)
                                .DefaultIndex("prospects")
                                .ThrowExceptions();

            return new ElasticClient(settings);
        }
    }
}
