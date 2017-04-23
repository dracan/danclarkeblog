using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace DanClarkeBlog.Functions.ScheduledSync
{
    public class ScheduledSyncFunction
    {
        public static async Task Run(TimerInfo myTimer, TraceWriter log)
        {
            log.Info("Starting Dropbox Sync ...");

            var ct = CancellationToken.None;
            var container = FunctionBootstrapper.Init(log);
            var notificationTarget = container.Resolve<INotificationTarget>();

            try
            {
                var sourceRepo = container.ResolveNamed<IBlogPostRepository>("Dropbox");
                var destRepo = container.ResolveNamed<IBlogPostRepository>("SqlServer");

                var helper = container.Resolve<SyncHelper>();

                await helper.SynchronizeBlogPostsAsync(sourceRepo, destRepo, false, ct);

                await notificationTarget.SendMessageAsync("A full scheduled synchonization has just successfully completed", ct);

                log.Info($"Finished dropbox sync");
            }
            catch(Exception ex)
            {
                await notificationTarget.SendMessageAsync($"An exception occurred in the full scheduled synchonization: {ex}", ct);
                throw;
            }
        }
    }
}
