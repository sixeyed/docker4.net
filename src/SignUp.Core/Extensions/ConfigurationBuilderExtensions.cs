using Microsoft.Extensions.Configuration;

namespace SignUp.Core
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddStandardSources(this IConfigurationBuilder builder)
        {
            return builder.AddJsonFile("appsettings.json")
                          .AddJsonFile("configs/config.json", optional: true)
                          .AddJsonFile("secrets/secret.json", optional: true)
                          .AddEnvironmentVariables();
        }
    }
}
