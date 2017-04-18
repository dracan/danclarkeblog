using System;
using Autofac;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using NLog;
using ILogger = DanClarkeBlog.Core.Helpers.ILogger;

namespace DanClarkeBlog.Core.Tests
{
    internal static class TestBootstrapper
    {
        public static IContainer Init()
        {
            var settings = new Settings
                           {
                               DropboxAccessToken = Environment.GetEnvironmentVariable("DropboxAccessToken"),
                               BlogSqlConnectionString = Environment.GetEnvironmentVariable("BlogSqlConnectionString"),
                               AzureStorageConnectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString"),
                               MaxResizedImageSize = int.Parse(Environment.GetEnvironmentVariable("MaxResizedImageSize") ?? "0"),
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
            builder.Register<ILogger>(x => new NLogLoggerImpl(LogManager.GetLogger("")));

            return builder.Build();
        }
    }
}
