using System;
using Autofac;
using DanClarkeBlog.Core;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using DanClarkeBlog.Tasks.Tasks;
using Serilog;

namespace DanClarkeBlog.Tasks
{
    internal static class TasksBootstrapper
    {
        public static IContainer Init()
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

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .CreateLogger();

            var builder = new ContainerBuilder();

            builder.Register(_ => settings);
            builder.RegisterType<BlogPostSqlServerRepository>().Named<IBlogPostRepository>("SqlServer");
            builder.RegisterType<BlogPostDropboxRepository>().Named<IBlogPostRepository>("Dropbox");
            builder.RegisterType<BlogPostSummaryHelper>();
            builder.RegisterType<BlogPostMarkdownRenderer>().As<IBlogPostRenderer>();
            builder.RegisterType<AzureImageRepository>().As<IImageRepository>();
            builder.RegisterType<ImageResizer>().As<IImageResizer>();
            builder.RegisterType<SyncHelper>();
            builder.RegisterType<DropboxHelper>().As<IDropboxHelper>();
            builder.RegisterType<HttpClientHelper>().As<IHttpClientHelper>();
            builder.RegisterType<SlackNotificationTarget>().As<INotificationTarget>();
            builder.RegisterType<FeedGenerator>().As<IFeedGenerator>();
            builder.RegisterType<HashVerify>().As<IHashVerify>();
            builder.RegisterType<AzureBlobLockRepository>().As<ILockRepository>();
            builder.RegisterType<RabbitMqMessageQueue>().As<IMessageQueue>();

            builder.RegisterType<SyncTask>();

            return builder.Build();
        }
    }
}
