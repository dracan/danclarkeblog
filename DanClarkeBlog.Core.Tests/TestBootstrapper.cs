using System;
using Autofac;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using NLog;
using NLog.Config;
using NLog.Targets;
using ILogger = DanClarkeBlog.Core.Helpers.ILogger;

namespace DanClarkeBlog.Core.Tests
{
    internal static class TestBootstrapper
    {
        public static IContainer Init(IHttpClientHelper httpClientHelper = null)
        {
            return Init<BlogPostSqlServerRepository>(httpClientHelper);
        }

        public static IContainer Init<TBlogPostRepository>(IHttpClientHelper httpClientHelper = null) where TBlogPostRepository : IBlogPostRepository
        {
            ConfigureNLog();

            var settings = new Settings
                           {
                               DropboxAccessToken = Environment.GetEnvironmentVariable("DropboxAccessToken"),
                               BlogSqlConnectionString = Environment.GetEnvironmentVariable("BlogSqlConnectionString"),
                               AzureStorageConnectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString"),
                               MaxResizedImageSize = int.Parse(Environment.GetEnvironmentVariable("MaxResizedImageSize") ?? "0"),
                               KeepAlivePingUri = Environment.GetEnvironmentVariable("KeepAlivePingUri"),
                               SlackNotificationUri = Environment.GetEnvironmentVariable("SlackNotificationUri"),
                               SiteHomeUri = Environment.GetEnvironmentVariable("SiteHomeUri"),
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
            builder.Register<ILogger>(x => new NLogLoggerImpl(LogManager.GetLogger(""))).InstancePerDependency();

            if (httpClientHelper == null)
                builder.RegisterType<HttpClientHelper>().As<IHttpClientHelper>();
            else
                builder.Register(x => httpClientHelper).As<IHttpClientHelper>();

            return builder.Build();
        }

        private static void ConfigureNLog()
        {
            var config = new LoggingConfiguration();

            // Console rule
            var target = new ConsoleTarget();
            config.AddTarget("console", target);
            var consoleRule = new LoggingRule("*", LogLevel.Trace, target);

            config.LoggingRules.Add(consoleRule);

            LogManager.Configuration = config;
        }
    }
}
