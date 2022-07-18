using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using DanClarkeBlog.Core.Helpers;
using DanClarkeBlog.Core.Models;
using DanClarkeBlog.Core.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace DanClarkeBlog.Core.Tests.Respositories
{
    public class BlogPostAzureSqlRepositoryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public BlogPostAzureSqlRepositoryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private const string SkipTest = "Manual"; // Make me null temporarily to run these tests. Ensure pointing at local database!

        /// <summary>
        /// This is just to manually run the Azure SQL code during dev. It's not an automated test.
        /// </summary>
        [Fact(Skip = SkipTest), Trait("Category", "Manual")]
        public async Task AddPost()
        {
            var container = TestBootstrapper.Init();

            var post = new BlogPost
                       {
                           Id = Guid.NewGuid(),
                           Title = "My Post",
                           Route = "/mypost",
                           HtmlText = "<body>Post Body</body>",
                           HtmlShortText = "short body",
                           PublishDate = new DateTime(2017, 03, 07)
                       };

            var repo = container.ResolveNamed<IBlogPostRepository>("SqlServer");

            await repo.AddOrUpdateAsync(post, CancellationToken.None);
        }

        /// <summary>
        /// This is just to manually run the Azure SQL code during dev. It's not an automated test.
        /// </summary>
        [Fact(Skip = SkipTest), Trait("Category", "Manual")]
        public async Task GetPosts()
        {
            var container = TestBootstrapper.Init();

            var repo = container.ResolveNamed<IBlogPostRepository>("SqlServer");

            var posts = await repo.GetAllAsync(null, CancellationToken.None);

            foreach(var post in posts)
            {
                _testOutputHelper.WriteLine(post.Title);
            }
        }

        /// <summary>
        /// This is just to manually run the Azure SQL code during dev. It's not an automated test.
        /// </summary>
        [Fact(Skip = SkipTest), Trait("Category", "Manual")]
        public async Task MigrateAsync()
        {
            var container = TestBootstrapper.Init();

            var destRepo = container.ResolveNamed<IBlogPostRepository>("SqlServer") as BlogPostSqlServerRepository;

            await destRepo!.UpdateDatabaseAsync(CancellationToken.None);
        }

        /// <summary>
        /// This is just to manually run the Azure SQL code during dev. It's not an automated test.
        /// </summary>
        [Fact(Skip = SkipTest), Trait("Category", "Manual")]
        public async Task RunSync()
        {
            var container = TestBootstrapper.Init();

            var sourceRepo = container.ResolveNamed<IBlogPostRepository>("Dropbox");
            var destRepo = (BlogPostSqlServerRepository)container.ResolveNamed<IBlogPostRepository>("SqlServer");

            //destRepo.CreateDatabase();
            //await destRepo.UpdateDatabaseAsync(CancellationToken.None);

            var helper = container.Resolve<SyncHelper>();

            await helper.SynchronizeBlogPostsAsync(sourceRepo, destRepo, false, null, CancellationToken.None);
        }

        /// <summary>
        /// This is just to manually run the Azure SQL code during dev. It's not an automated test.
        /// </summary>
        [Fact(Skip = SkipTest), Trait("Category", "Manual")]
        public async Task IncrementalSyncTest()
        {
            var container = TestBootstrapper.Init();

            var sourceRepo = container.ResolveNamed<IBlogPostRepository>("Dropbox");
            var destRepo = (BlogPostSqlServerRepository)container.ResolveNamed<IBlogPostRepository>("SqlServer");

            //destRepo.CreateDatabase();
            //await destRepo.UpdateDatabaseAsync(CancellationToken.None);

            var helper = container.Resolve<SyncHelper>();

            // Grab this from the database, then make a change in Dropbox to manually test the incremental sync
            const string testCursor = "AAEr_QdeOumu08_iV7Ah9wDoQ8zbWmBs2dtILwYs3e9ij-ZmBIT-93Ova_DP37JJG3sPeetCRZDYICDWc0BtmLAP03LvTw6oqHhSI8EF_YLZxInkyEr9-G5fhpLdVTiO4Rui9FmZTnEAakQB8437HUBb";

            await helper.SynchronizeBlogPostsAsync(sourceRepo, destRepo, true, testCursor, CancellationToken.None);
        }
    }
}