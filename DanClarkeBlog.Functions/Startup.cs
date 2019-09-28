using System;
using DanClarkeBlog.Core;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(DanClarkeBlog.Functions.Startup))]

namespace DanClarkeBlog.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var settings = new Settings
            {
                DropboxAccessToken = Environment.GetEnvironmentVariable("Blog__DropboxAccessToken"),
                BlogSqlConnectionString = Environment.GetEnvironmentVariable("Blog__BlogSqlConnectionString"),
                AzureStorageConnectionString = Environment.GetEnvironmentVariable("Blog__AzureStorageConnectionString"),
                MaxResizedImageSize = int.Parse(Environment.GetEnvironmentVariable("Blog__MaxResizedImageSize") ?? "0"),
                KeepAlivePingUri = Environment.GetEnvironmentVariable("Blog__KeepAlivePingUri"),
                SlackNotificationUri = Environment.GetEnvironmentVariable("Blog__SlackNotificationUri"),
                PostPreviewLength = int.Parse(Environment.GetEnvironmentVariable("Blog__PostPreviewLength") ?? "200"),
                BaseImageUri  = Environment.GetEnvironmentVariable("Blog__BaseImageUri"),
                RabbitMQServer = Environment.GetEnvironmentVariable("Blog__RabbitMQServer"),
                RabbitMQUser = Environment.GetEnvironmentVariable("Blog__RabbitMQUser"),
                RabbitMQPass = Environment.GetEnvironmentVariable("Blog__RabbitMQPass"),
            };

            builder.Services.AddSingleton(settings);
            builder.Services.AddSingleton<IBlogPostTargetRepository, BlogPostSqlServerRepository>();
            builder.Services.AddSingleton<IBlogPostSourceRepository, BlogPostDropboxRepository>();
            builder.Services.AddSingleton<BlogPostSummaryHelper>();
            builder.Services.AddSingleton<IBlogPostRenderer, BlogPostMarkdownRenderer>();
            builder.Services.AddSingleton<IImageRepository, AzureImageRepository>();
            builder.Services.AddSingleton<IImageResizer, ImageResizer>();
            builder.Services.AddSingleton<SyncHelper>();
            builder.Services.AddSingleton<IDropboxHelper, DropboxHelper>();
            builder.Services.AddSingleton<IHttpClientHelper, HttpClientHelper>();
            builder.Services.AddSingleton<INotificationTarget, SlackNotificationTarget>();
            builder.Services.AddSingleton<IFeedGenerator, FeedGenerator>();
            builder.Services.AddSingleton<IHashVerify, HashVerify>();
            builder.Services.AddSingleton<ILockRepository, AzureBlobLockRepository>();
            builder.Services.AddSingleton<IMessageQueue, AzureStorageQueue>();
        }
    }
}