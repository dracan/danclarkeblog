using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using DanClarkeBlog.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace DanClarkeBlog.Functions.KeepAlive
{
    public class KeepAliveFunction
    {
        public static async Task Run(TimerInfo myTimer, TraceWriter log)
        {
            var container = Bootstrapper.Init(log);
            var settings = container.Resolve<Settings>();

            using (var client = new HttpClient())
            {
                await client.GetAsync(settings.KeepAlivePingUri);
            }
        }
    }
}
