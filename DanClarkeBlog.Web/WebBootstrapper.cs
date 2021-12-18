using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DanClarkeBlog.Web
{
    internal static class WebBootstrapper
    {
        public static void Init(IServiceCollection services)
        {
            services.AddScoped<IBlogPostRepository, BlogPostSqlServerRepository>();
            services.AddSingleton<BlogPostSummaryHelper>();
            services.AddSingleton<IBlogPostRenderer, BlogPostMarkdownRenderer>();
            services.AddScoped<IImageRepository, AzureImageRepository>();
            services.AddSingleton<IImageResizer, ImageResizer>();
            services.AddScoped<SyncHelper>();
            services.AddSingleton<IDropboxHelper, DropboxHelper>();
            services.AddSingleton<IHttpClientHelper, HttpClientHelper>();
            services.AddSingleton<INotificationTarget, SlackNotificationTarget>();
            services.AddScoped<IFeedGenerator, FeedGenerator>();
            services.AddSingleton<IHashVerify, HashVerify>();
            services.AddSingleton<IMessageQueue, AzureStorageQueue>();
            services.AddSingleton<ILockRepository, AzureBlobLockRepository>();
            services.AddSingleton<ISearchHelper, AzureSearchHelper>();
        }
    }
}
