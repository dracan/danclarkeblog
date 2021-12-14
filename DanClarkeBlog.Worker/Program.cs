using DanClarkeBlog.Core;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using DanClarkeBlog.Worker;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        var settings = new Settings
        {
            DropboxAccessToken = Environment.GetEnvironmentVariable("Blog__DropboxAccessToken"),
            BlogSqlConnectionString = Environment.GetEnvironmentVariable("Blog__BlogSqlConnectionString"),
            AzureStorageConnectionString = Environment.GetEnvironmentVariable("Blog__AzureStorageConnectionString"),
            AzureServiceBusConnectionString = Environment.GetEnvironmentVariable("Blog__AzureServiceBusConnectionString"),
            MaxResizedImageSize = int.Parse(Environment.GetEnvironmentVariable("Blog__MaxResizedImageSize") ?? "0"),
            KeepAlivePingUri = Environment.GetEnvironmentVariable("Blog__KeepAlivePingUri"),
            SlackNotificationUri = Environment.GetEnvironmentVariable("Blog__SlackNotificationUri"),
            PostPreviewLength = int.Parse(Environment.GetEnvironmentVariable("Blog__PostPreviewLength") ?? "200"),
            BaseImageUri = Environment.GetEnvironmentVariable("Blog__BaseImageUri"),
        };

        services.AddSingleton(settings);
        services.AddSingleton<IBlogPostTargetRepository, BlogPostSqlServerRepository>();
        services.AddSingleton<IBlogPostSourceRepository, BlogPostDropboxRepository>();
        services.AddSingleton<BlogPostSummaryHelper>();
        services.AddSingleton<IBlogPostRenderer, BlogPostMarkdownRenderer>();
        services.AddSingleton<IImageRepository, AzureImageRepository>();
        services.AddSingleton<IImageResizer, ImageResizer>();
        services.AddSingleton<SyncHelper>();
        services.AddSingleton<IDropboxHelper, DropboxHelper>();
        services.AddSingleton<IHttpClientHelper, HttpClientHelper>();
        services.AddSingleton<INotificationTarget, SlackNotificationTarget>();
        services.AddSingleton<IFeedGenerator, FeedGenerator>();
        services.AddSingleton<IHashVerify, HashVerify>();
        services.AddSingleton<ILockRepository, AzureBlobLockRepository>();
        services.AddSingleton<IMessageQueue, AzureStorageQueue>();

        services.AddHostedService<WorkerService>();

        services.AddMediatR(typeof(Program));
        services.AddLogging(builder => builder.AddSimpleConsole());
    })
    .UseConsoleLifetime();

var host = builder.Build();

await host.RunAsync();
