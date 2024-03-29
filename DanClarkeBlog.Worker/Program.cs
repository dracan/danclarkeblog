﻿using DanClarkeBlog.Core;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using DanClarkeBlog.Worker;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

IConfiguration? config = null;

var builder = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration((_, configBuilder) =>
    {
        var keyVaultUri = configBuilder.Build()["KeyVaultUri"];

        if (!string.IsNullOrWhiteSpace(keyVaultUri))
            configBuilder.AddAzureKeyVault(keyVaultUri, new DefaultKeyVaultSecretManager());

        config = configBuilder.Build();
    })
    .ConfigureServices(services =>
    {
        services.Configure<Settings>(config!.GetSection("Blog"));

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
        services.AddSingleton<IMessageQueue, AzureServiceBusPublisher>();

        services.AddHostedService<WorkerService>();

        services.AddApplicationInsightsTelemetryWorkerService();

        services.AddMediatR(typeof(Program));
        services.AddLogging(logging => logging.AddApplicationInsights());
    })
    .UseConsoleLifetime();

var host = builder.Build();

await host.RunAsync();
