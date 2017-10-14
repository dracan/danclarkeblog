using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus.Messaging;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;

namespace DanClarkeBlog.Functions
{
    public static class ProcessDropboxChange
    {
        [FunctionName("ProcessDropboxChange")]
        public static async Task Run([ServiceBusTrigger("dropboxupdates", AccessRights.Listen, Connection = "danclarkeblog")] string message, TraceWriter log)
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

                await helper.SynchronizeBlogPostsAsync(sourceRepo, destRepo, true, null, CancellationToken.None);

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
