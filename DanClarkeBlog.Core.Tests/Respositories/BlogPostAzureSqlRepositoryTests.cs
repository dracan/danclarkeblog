using System;
using System.Threading;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Models;
using DanClarkeBlog.Core.Respositories;
using NUnit.Framework;

namespace DanClarkeBlog.Core.Tests.Respositories
{
    [TestFixture]
    public class BlogPostAzureSqlRepositoryTests
    {
        /// <summary>
        /// This is just to manually run the Azure SQL code during dev. It's not an automated test.
        /// </summary>
        [Test, Explicit]
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

            var repo = new BlogPostAzureSqlRepository(settings);

            repo.CreateDatabase();

            await repo.AddAsync(post, CancellationToken.None);
        }

        /// <summary>
        /// This is just to manually run the Azure SQL code during dev. It's not an automated test.
        /// </summary>
        [Test, Explicit]
        public async Task GetPosts()
        {
            var settings = new Settings
                           {
                               BlogSqlConnectionString = Environment.GetEnvironmentVariable("BlogSqlConnectionString"),
                           };

            var repo = new BlogPostAzureSqlRepository(settings);

            var posts = await repo.GetAllAsync(CancellationToken.None);

            foreach(var post in posts)
            {
                Console.WriteLine(post.Title);
            }
        }

        /// <summary>
        /// This is just to manually run the Azure SQL code during dev. It's not an automated test.
        /// </summary>
        [Test/*, Explicit*/]
        public async Task RunSync()
        {
            var settings = new Settings
                           {
                               DropboxAccessToken = Environment.GetEnvironmentVariable("DropboxAccessToken"),
                               BlogSqlConnectionString = Environment.GetEnvironmentVariable("BlogSqlConnectionString"),
                           };

            var blogPostRenderer = new BlogPostMarkdownRenderer();
            var blogPostSummaryHelper = new BlogPostSummaryHelper();

            var sourceRepo = new BlogPostDropboxRepository(blogPostRenderer, settings, blogPostSummaryHelper);
            var destRepo = new BlogPostAzureSqlRepository(settings);

            //destRepo.CreateDatabase();

            var helper = new SyncHelper();

            await helper.SynchronizeBlogPostsAsync(sourceRepo, destRepo, CancellationToken.None);
        }
    }
}