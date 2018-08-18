using System;
using Autofac;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using Serilog;

namespace DanClarkeBlog.Core.Tests
{
    internal static class TestBootstrapper
    {
        public static IContainer Init(IHttpClientHelper httpClientHelper = null)
        {
            return Init<BlogPostSqlServerRepository>(httpClientHelper);
        }

        private static IContainer Init<TBlogPostRepository>(IHttpClientHelper httpClientHelper = null) where TBlogPostRepository : IBlogPostRepository
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.LiterateConsole()
                .CreateLogger();

            var settings = new Settings
                           {
                               DropboxAccessToken = Environment.GetEnvironmentVariable("Blog__DropboxAccessToken"),
                               BlogSqlConnectionString = Environment.GetEnvironmentVariable("Blog__BlogSqlConnectionString"),
                               AzureStorageConnectionString = Environment.GetEnvironmentVariable("Blog__AzureStorageConnectionString"),
                               MaxResizedImageSize = int.Parse(Environment.GetEnvironmentVariable("Blog__MaxResizedImageSize") ?? "0"),
                               KeepAlivePingUri = Environment.GetEnvironmentVariable("Blog__KeepAlivePingUri"),
                               SlackNotificationUri = Environment.GetEnvironmentVariable("Blog__SlackNotificationUri"),
                               SiteHomeUri = Environment.GetEnvironmentVariable("Blog__SiteHomeUri"),
                               PostPreviewLength = int.Parse(Environment.GetEnvironmentVariable("Blog__PostPreviewLength") ?? "200"),
                               BaseImageUri = Environment.GetEnvironmentVariable("Blog__BaseImageUri"),
            };

            var builder = new ContainerBuilder();

            builder.Register(_ => settings);
            builder.RegisterType<TBlogPostRepository>().As<IBlogPostRepository>();
            builder.RegisterType<BlogPostSqlServerRepository>().Named<IBlogPostRepository>("SqlServer");
            builder.RegisterType<BlogPostDropboxRepository>().Named<IBlogPostRepository>("Dropbox");
            builder.RegisterType<BlogPostSummaryHelper>();
            builder.RegisterType<BlogPostMarkdownRenderer>().As<IBlogPostRenderer>();
            builder.RegisterType<AzureImageRepository>().As<IImageRepository>();
            builder.RegisterType<ImageResizer>().As<IImageResizer>();
            builder.RegisterType<SyncHelper>();
            builder.RegisterType<DropboxHelper>().As<IDropboxHelper>();
            builder.RegisterType<SlackNotificationTarget>().As<INotificationTarget>();
            builder.RegisterType<FeedGenerator>().As<IFeedGenerator>();
            builder.RegisterType<AzureBlobLockRepository>().As<ILockRepository>();

            if (httpClientHelper == null)
                builder.RegisterType<HttpClientHelper>().As<IHttpClientHelper>();
            else
                builder.Register(x => httpClientHelper).As<IHttpClientHelper>();

            return builder.Build();
        }
    }
}
