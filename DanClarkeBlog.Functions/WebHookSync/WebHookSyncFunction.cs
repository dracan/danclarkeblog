using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using Microsoft.Azure.WebJobs.Host;

namespace DanClarkeBlog.Functions.WebHookSync
{
    public class WebHookSyncFunction
    {
        public static async Task Run(HttpRequestMessage req, TraceWriter log)
        {
            log.Info($"Starting Dropbox Sync ...");

            var settings = new Settings
            {
                DropboxAccessToken = Environment.GetEnvironmentVariable("DropboxAccessToken"),
                BlogSqlConnectionString = Environment.GetEnvironmentVariable("BlogSqlConnectionString"),
                AzureStorageConnectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString"),
            };

            var blogPostRenderer = new BlogPostMarkdownRenderer();
            var blogPostSummaryHelper = new BlogPostSummaryHelper();
            var imageRepository = new AzureImageRepository(settings);
            var dropboxHelper = new DropboxHelper(settings, new HttpClientHelper());

            var sourceRepo = new BlogPostDropboxRepository(blogPostRenderer, settings, blogPostSummaryHelper, imageRepository, dropboxHelper);
            var destRepo = new BlogPostSqlServerRepository(settings);

            var helper = new SyncHelper();

            await helper.SynchronizeBlogPostsAsync(sourceRepo, destRepo, CancellationToken.None);

            log.Info($"Finished dropbox sync");
        }
    }
}
