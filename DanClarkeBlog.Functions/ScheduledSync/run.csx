#r "DanClarkeBlog.Core.dll"

using System;
using DanClarkeBlog.Core;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Respositories;
using System.Configuration;
using System.Threading;

public static async Task Run(TimerInfo myTimer, TraceWriter log)
{
    log.Info($"Starting Dropbox Sync ...");

    var helper = new SyncHelper();

    var settings = new Settings
    {
        DropboxAccessToken = ConfigurationManager.AppSettings["DropboxAccessToken"],
        BlogSqlConnectionString = ConfigurationManager.AppSettings["BlogSqlConnectionString"],
    };

    var blogPostRenderer = new BlogPostMarkdownRenderer();
    var blogPostSummaryHelper = new BlogPostSummaryHelper();

    var sourceRepo = new BlogPostDropboxRepository(blogPostRenderer, settings, blogPostSummaryHelper);
    var destRepo = new BlogPostAzureSqlRepository(settings);

    await helper.SynchronizeBlogPostsAsync(sourceRepo, destRepo, CancellationToken.None);
}