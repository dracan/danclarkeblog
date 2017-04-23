using System;
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
            var ct = CancellationToken.None;
            var container = FunctionBootstrapper.Init(log);
            var notificationTarget = container.Resolve<INotificationTarget>();

            try
            {
                log.Info($"Found message on queue: {message}");

                if (message != "INCREMENTAL_DROPBOX_UPDATE")
                {
                    await notificationTarget.SendMessageAsync($@"Received message on queue which wasn't ""INCREMENTAL_DROPBOX_UPDATE"" {message}", ct);
                    return;
                }

                var sourceRepo = container.ResolveNamed<IBlogPostRepository>("Dropbox");
                var destRepo = container.ResolveNamed<IBlogPostRepository>("SqlServer");

                var helper = container.Resolve<SyncHelper>();

                await helper.SynchronizeBlogPostsAsync(sourceRepo, destRepo, true, CancellationToken.None);

                log.Info("Finished incremental dropbox sync");
            }
            catch (Exception ex)
            {
                await notificationTarget.SendMessageAsync($"An exception occurred in the ProcessDropboxChange Azure Function: {ex}", ct);
                throw;
            }
        }
    }
}
