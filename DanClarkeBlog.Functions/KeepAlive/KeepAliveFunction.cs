using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DanClarkeBlog.Core;
using DanClarkeBlog.Core.Helpers;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace DanClarkeBlog.Functions.KeepAlive
{
    public class KeepAliveFunction
    {
        public static async Task Run(TimerInfo myTimer, TraceWriter log)
        {
            var ct = CancellationToken.None;
            var container = FunctionBootstrapper.Init(log);
            var notificationTarget = container.Resolve<INotificationTarget>();

            try
            {
                var settings = container.Resolve<Settings>();

                using (var client = new HttpClient())
                {
                    await client.GetAsync(settings.KeepAlivePingUri, ct);
                }
            }
            catch (Exception ex)
            {
                await notificationTarget.SendMessageAsync($"An exception occurred in the keep alive function: {ex}", ct);
                throw;
            }
        }
    }
}
