using Microsoft.Extensions.Configuration;
using SignUp.Core;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using prom = Prometheus;

namespace SignUp.Web
{
    public class Metrics : HttpTaskAsyncHandler
    {

        public override async Task ProcessRequestAsync(HttpContext context)
        {
            if (Config.Current.GetValue<bool>("Metrics:Server:Enabled"))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    await prom.Metrics.DefaultRegistry.CollectAndExportAsTextAsync(stream);
                    stream.Position = 0;
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var metrics = await reader.ReadToEndAsync();
                        context.Response.ContentType = "text/plain";
                        context.Response.ContentEncoding = Encoding.UTF8;
                        context.Response.Write(metrics);
                    }
                }
            }
            else
            {
                context.Response.StatusCode = 404;
            }
        }

        public override bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}