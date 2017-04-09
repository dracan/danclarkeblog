using System;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Models;
using DanClarkeBlog.Core.Repositories;
using Xunit;

namespace DanClarkeBlog.Core.Tests.Respositories
{
    public class BlogPostAzureSqlRepositoryTests
    {
        /// <summary>
        /// This is just to manually run the Azure SQL code during dev. It's not an automated test.
        /// </summary>
        [Fact, Trait("Category", "Manual")]
        public async Task AddPost()
        {
            var settings = new Settings
                           {
                               BlogSqlConnectionString = Environment.GetEnvironmentVariable("BlogSqlConnectionString"),
                           };

            var post = new BlogPost
                       {
                           Title = "My Post",
                           Route = "/mypost",
                           HtmlText = "<body>Post Body</body>",
                           HtmlShortText = "short body",
                           PublishDate = new DateTime(2017, 03, 07)
                       };

            var repo = new BlogPostSqlServerRepository(settings);

            repo.CreateDatabase();

            await repo.AddAsync(post, CancellationToken.None);
        }

        /// <summary>
        /// This is just to manually run the Azure SQL code during dev. It's not an automated test.
        /// </summary>
        [Fact, Trait("Category", "Manual")]
        public async Task GetPosts()
        {
            var settings = new Settings
                           {
                               BlogSqlConnectionString = Environment.GetEnvironmentVariable("BlogSqlConnectionString"),
                           };

            var repo = new BlogPostSqlServerRepository(settings);

            var posts = await repo.GetAllAsync(CancellationToken.None);

            foreach(var post in posts)
            {
                Console.WriteLine(post.Title);
            }
        }

        /// <summary>
        /// This is just to manually run the Azure SQL code during dev. It's not an automated test.
        /// </summary>
        [Fact, Trait("Category", "Manual")]
        public async Task MigrateAsync()
        {
            var settings = new Settings
                           {
                               DropboxAccessToken = Environment.GetEnvironmentVariable("DropboxAccessToken"),
                               BlogSqlConnectionString = Environment.GetEnvironmentVariable("BlogSqlConnectionString"),
                               AzureStorageConnectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString"),
                           };

            var destRepo = new BlogPostSqlServerRepository(settings);

            await destRepo.UpdateDatabaseAsync(CancellationToken.None);
        }

        /// <summary>
        /// This is just to manually run the Azure SQL code during dev. It's not an automated test.
        /// </summary>
        [Fact, Trait("Category", "Manual")]
        public async Task RunSync()
        {
            var settings = new Settings
                           {
                               DropboxAccessToken = Environment.GetEnvironmentVariable("DropboxAccessToken"),
                               BlogSqlConnectionString = Environment.GetEnvironmentVariable("BlogSqlConnectionString"),
                               AzureStorageConnectionString = Environment.GetEnvironmentVariable("AzureStorageConnectionString"),
                           };

            var blogPostRenderer = new BlogPostMarkdownRenderer();
            var blogPostSummaryHelper = new BlogPostSummaryHelper();
            var imageRepository = new AzureImageRepository(settings);
            var dropboxHelper = new DropboxHelper(settings, new HttpClientHelper());
            var imageResizer = new ImageResizer();

            var sourceRepo = new BlogPostDropboxRepository(blogPostRenderer, settings, blogPostSummaryHelper, imageRepository, dropboxHelper, imageResizer);
            var destRepo = new BlogPostSqlServerRepository(settings);

            //destRepo.CreateDatabase();
            await destRepo.UpdateDatabaseAsync(CancellationToken.None);

            var helper = new SyncHelper();

            await helper.SynchronizeBlogPostsAsync(sourceRepo, destRepo, false, CancellationToken.None);
        }
    }
}