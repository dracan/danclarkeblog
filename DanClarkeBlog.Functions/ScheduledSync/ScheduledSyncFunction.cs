using System;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core;
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
            log.Info($"Starting Dropbox Sync ...");

            var settings = new Settings
            {
                DropboxAccessToken = Environment.GetEnvironmentVariable("DropboxAccessToken"),
                BlogSqlConnectionString = Environment.GetEnvironmentVariable("BlogSqlConnectionString"),
                AzureStorageConnectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString"),
            };

            var logger = new TraceLogLoggerImpl(log);

            var blogPostRenderer = new BlogPostMarkdownRenderer();
            var blogPostSummaryHelper = new BlogPostSummaryHelper();
            var imageRepository = new AzureImageRepository(settings, logger);
            var dropboxHelper = new DropboxHelper(settings, new HttpClientHelper());
            var imageResizer = new ImageResizer();

            var sourceRepo = new BlogPostDropboxRepository(blogPostRenderer, settings, blogPostSummaryHelper, imageRepository, dropboxHelper, imageResizer, logger);
            var destRepo = new BlogPostSqlServerRepository(settings, logger);

            var helper = new SyncHelper();

            await helper.SynchronizeBlogPostsAsync(sourceRepo, destRepo, dropboxHelper, false, logger, CancellationToken.None);

            log.Info($"Finished dropbox sync");
        }
    }
}
