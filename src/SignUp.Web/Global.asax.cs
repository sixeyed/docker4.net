using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using prom = Prometheus;
using Prometheus.AspNet;
using SignUp.Core;
using SignUp.Messaging;
using SignUp.Model;
using SignUp.Model.Initializers;
using SignUp.Web.Logging;
using SignUp.Web.ProspectSave;
using SignUp.Web.ReferenceData;

namespace SignUp.Web
{
    public class Global : HttpApplication
    {
        public static ServiceProvider ServiceProvider { get; private set; }
        public static IHttpModule Module = new PrometheusHttpRequestModule();

        private static prom.Gauge _InfoGauge;

        static Global()
        {
            ServiceProvider = new ServiceCollection()
                .AddSingleton(Config.Current)
                .AddSingleton<MessageQueue>()
                .AddTransient<DatabaseReferenceDataLoader>()
                .AddTransient<ApiReferenceDataLoader>()
                .AddTransient<SynchronousProspectSaveHandler>()
                .AddTransient<AsynchronousProspectSaveHandler>()
                .BuildServiceProvider();
        }

        void Application_Start(object sender, EventArgs e)
        {
            Log.Info("Application_Start");

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Database.SetInitializer<SignUpContext>(new StaticDataInitializer());
            EnsureDatabaseCreated();
            SignUp.PreloadStaticDataCache();
        }

        public override void Init()
        {
            base.Init();
            if (Config.Current.GetValue<bool>("Metrics:Server:Enabled"))
            {
                Module.Init(this);
                _InfoGauge = prom.Metrics.CreateGauge("app_info", "Application info", "netfx_version", "version");
                _InfoGauge.Labels(AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName, "20.11").Set(1);
            }
        }

        private static void EnsureDatabaseCreated()
        {
            Log.Info("Ensuring database exists");
            try
            {
                var connectionString = ConfigurationManager.ConnectionStrings["SignUpDb"].ConnectionString.ToLower();
                var server = connectionString.Split(';').First(x => x.StartsWith("server=")).Split('=')[1];
                Log.Debug($"Connecting to database server: {server}");
                using (var context = new SignUpContext())
                {
                    context.Countries.ToListAsync().Wait();
                }
                Log.Info("Database connection is OK");
            }
            catch(Exception ex)
            {
                Log.Fatal($"Database connection failed, exception: {ex}");
            }            
        }
    }
}