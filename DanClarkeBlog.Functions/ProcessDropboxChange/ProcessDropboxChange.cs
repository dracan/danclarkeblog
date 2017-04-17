using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using Microsoft.Azure.WebJobs.Host;

namespace DanClarkeBlog.Functions.ProcessDropboxChange
{
    public class ProcessDropboxChangeFunction
    {
        public static async Task Run(string message, TraceWriter log)
        {
            log.Info($"Found message on queue: {message}");

            if (message != "INCREMENTAL_DROPBOX_UPDATE")
            {
                return;
            }

            var container = Bootstrapper.Init(log);

            var sourceRepo = container.ResolveNamed<IBlogPostRepository>("Dropbox");
            var destRepo = container.ResolveNamed<IBlogPostRepository>("SqlServer");

            var helper = container.Resolve<SyncHelper>();

            await helper.SynchronizeBlogPostsAsync(sourceRepo, destRepo, true, CancellationToken.None);

            log.Info("Finished dropbox sync");
        }
    }
}
