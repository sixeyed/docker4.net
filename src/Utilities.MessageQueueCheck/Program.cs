using Microsoft.Extensions.Configuration;
using NATS.Client;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Utilities.MessageQueueCheck
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("configs/config.json", optional: true)
                .AddJsonFile("secrets/secret.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var exitCode = 1;
            try
            {
                using (var connection = new ConnectionFactory().CreateConnection(config["MessageQueue:Url"]))
                {
                    exitCode = 0;
                    Console.WriteLine($"MessageQueueCheck: OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MessageQueueCheck: error. Exception {ex.Message}");
            }
            return exitCode;

        }
    }
}
