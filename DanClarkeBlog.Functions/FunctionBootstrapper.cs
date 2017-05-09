using System;
using Autofac;
using DanClarkeBlog.Core;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using Microsoft.Azure.WebJobs.Host;

namespace DanClarkeBlog.Functions
{
    internal static class FunctionBootstrapper
    {
        public static IContainer Init(TraceWriter traceWriter)
        {
            var settings = new Settings
                           {
                               DropboxAccessToken = Environment.GetEnvironmentVariable("Blog:DropboxAccessToken"),
                               BlogSqlConnectionString = Environment.GetEnvironmentVariable("Blog:BlogSqlConnectionString"),
                               AzureStorageConnectionString = Environment.GetEnvironmentVariable("Blog:AzureStorageConnectionString"),
                               MaxResizedImageSize = int.Parse(Environment.GetEnvironmentVariable("Blog:MaxResizedImageSize") ?? "0"),
                               KeepAlivePingUri = Environment.GetEnvironmentVariable("Blog:KeepAlivePingUri"),
                               SlackNotificationUri = Environment.GetEnvironmentVariable("Blog:SlackNotificationUri"),
                               SiteHomeUri = Environment.GetEnvironmentVariable("Blog:SiteHomeUri"),
                               DropboxAppSecret = Environment.GetEnvironmentVariable("Blog:DropboxAppSecret"),
                               ProfilePicUri = Environment.GetEnvironmentVariable("Blog:ProfilePicUri"),
                               PostPreviewLength = int.Parse(Environment.GetEnvironmentVariable("Blog:PostPreviewLength") ?? "200"),
                               BaseImageUri  = Environment.GetEnvironmentVariable("Blog:BaseImageUri"),
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
            builder.RegisterType<FeedGenerator>().As<IFeedGenerator>();
            builder.RegisterType<HashVerify>().As<IHashVerify>();
            builder.Register<ILogger>(x => new TraceLogLoggerImpl(traceWriter));

            return builder.Build();
        }
    }
}
