#r "DanClarkeBlog.Core.dll"
#r "Microsoft.WindowsAzure.Storage"

using System;
using DanClarkeBlog.Core;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using System.Configuration;
using System.Threading;

public static async Task Run(TimerInfo myTimer, TraceWriter log)
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