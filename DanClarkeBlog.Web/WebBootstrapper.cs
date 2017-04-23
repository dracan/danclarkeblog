using Autofac;
using Autofac.Extensions.DependencyInjection;
using DanClarkeBlog.Core;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DanClarkeBlog.Web
{
    internal static class WebBootstrapper
    {
        public static IContainer Init(IServiceCollection services, Settings settings, ILogger logger)
        {
            var builder = new ContainerBuilder();

            builder.Register(_ => settings);
            builder.RegisterType<BlogPostSqlServerRepository>().As<IBlogPostRepository>();
            builder.RegisterType<BlogPostSummaryHelper>();
            builder.RegisterType<BlogPostMarkdownRenderer>().As<IBlogPostRenderer>();
            builder.RegisterType<AzureImageRepository>().As<IImageRepository>();
            builder.RegisterType<ImageResizer>().As<IImageResizer>();
            builder.RegisterType<SyncHelper>();
            builder.RegisterType<DropboxHelper>().As<IDropboxHelper>();
            builder.RegisterType<HttpClientHelper>().As<IHttpClientHelper>();
            builder.RegisterType<SlackNotificationTarget>().As<INotificationTarget>();
            builder.RegisterType<FeedGenerator>().As<IFeedGenerator>();
            builder.Register(x => logger);

            builder.Populate(services);

            return builder.Build();
        }
    }
}
