using Microsoft.Extensions.Configuration;

namespace SignUp.Core
{
    public class Config
    {
        public static IConfiguration Current { get; private set; }

        static Config()
        {
            var builder = new ConfigurationBuilder();
            builder.AddStandardSources();
            Current = builder.Build();
        }
    }
}
