using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
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
            var container = TestBootstrapper.Init();

            var post = new BlogPost
                       {
                           Title = "My Post",
                           Route = "/mypost",
                           HtmlText = "<body>Post Body</body>",
                           HtmlShortText = "short body",
                           PublishDate = new DateTime(2017, 03, 07)
                       };

            var repo = container.ResolveNamed<IBlogPostRepository>("SqlServer");

            await repo.AddAsync(post, CancellationToken.None);
        }

        /// <summary>
        /// This is just to manually run the Azure SQL code during dev. It's not an automated test.
        /// </summary>
        [Fact, Trait("Category", "Manual")]
        public async Task GetPosts()
        {
            var container = TestBootstrapper.Init();

            var repo = container.ResolveNamed<IBlogPostRepository>("SqlServer");

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
            var container = TestBootstrapper.Init();

            var destRepo = container.ResolveNamed<IBlogPostRepository>("SqlServer") as BlogPostSqlServerRepository;

            await destRepo.UpdateDatabaseAsync(CancellationToken.None);
        }

        /// <summary>
        /// This is just to manually run the Azure SQL code during dev. It's not an automated test.
        /// </summary>
        [Fact, Trait("Category", "Manual")]
        public async Task RunSync()
        {
            var container = TestBootstrapper.Init();

            var sourceRepo = container.ResolveNamed<IBlogPostRepository>("Dropbox");
            var destRepo = container.ResolveNamed<IBlogPostRepository>("SqlServer");

            //destRepo.CreateDatabase();
            //await destRepo.UpdateDatabaseAsync(CancellationToken.None);

            var helper = container.Resolve<SyncHelper>();

            await helper.SynchronizeBlogPostsAsync(sourceRepo, destRepo, false, CancellationToken.None);
        }
    }
}