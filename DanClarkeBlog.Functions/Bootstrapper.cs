using System;
using Autofac;
using DanClarkeBlog.Core;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using Microsoft.Azure.WebJobs.Host;

namespace DanClarkeBlog.Functions
{
    internal static class Bootstrapper
    {
        public static IContainer Init(TraceWriter traceWriter)
        {
            var settings = new Settings
                           {
                               DropboxAccessToken = Environment.GetEnvironmentVariable("DropboxAccessToken"),
                               BlogSqlConnectionString = Environment.GetEnvironmentVariable("BlogSqlConnectionString"),
                               AzureStorageConnectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString"),
                               MaxResizedImageSize = int.Parse(Environment.GetEnvironmentVariable("MaxResizedImageSize") ?? "0"),
                               KeepAlivePingUri = Environment.GetEnvironmentVariable("KeepAlivePingUri"),
                               SlackNotificationUri = Environment.GetEnvironmentVariable("SlackNotificationUri"),
            };

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
            builder.Register<ILogger>(x => new TraceLogLoggerImpl(traceWriter));

            return builder.Build();
        }
    }
}
