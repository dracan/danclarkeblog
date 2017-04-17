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

            var container = Bootstrapper.Init(log);

            var sourceRepo = container.ResolveNamed<IBlogPostRepository>("Dropbox");
            var destRepo = container.ResolveNamed<IBlogPostRepository>("SqlServer");

            var helper = container.Resolve<SyncHelper>();

            await helper.SynchronizeBlogPostsAsync(sourceRepo, destRepo, false, CancellationToken.None);

            log.Info($"Finished dropbox sync");
        }
    }
}
