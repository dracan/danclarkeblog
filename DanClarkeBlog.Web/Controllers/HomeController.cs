using System;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Repositories;
using DanClarkeBlog.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading;
using DanClarkeBlog.Core.Helpers;
using Settings = DanClarkeBlog.Core.Settings;

namespace DanClarkeBlog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly Settings _settings;
        private readonly IFeedGenerator _feedGenerator;
        internal const int NumPostsPerPage = 10;
        internal const int NumRecentPosts = 5;

        public HomeController(IBlogPostRepository blogPostRepository, Settings _settings, IFeedGenerator feedGenerator)
        {
            _blogPostRepository = blogPostRepository;
            this._settings = _settings;
            _feedGenerator = feedGenerator;
        }

        public async Task<IActionResult> Index(string tag, int? page, CancellationToken cancellationToken)
        {
            var offset = ((page ?? 1) - 1) * NumPostsPerPage;

            var pagedPostsTask = _blogPostRepository.GetPublishedAsync(tag, offset, NumPostsPerPage, cancellationToken);
            var featuredPostsTask = _blogPostRepository.GetFeaturedAsync(cancellationToken);
            var recentPostsTask = _blogPostRepository.GetRecentAsync(NumRecentPosts, cancellationToken);
            var tagsTask = _blogPostRepository.GetTagCountsAsync(cancellationToken);

            var pagedPostsResults = await pagedPostsTask;
            var featuredPosts = await featuredPostsTask;
            var recentPosts = await recentPostsTask;
            var tags = await tagsTask;

            return View(new HomeViewModel
            {
                FeaturedPosts = featuredPosts,
                RecentPosts = recentPosts,
                Posts = pagedPostsResults.Posts,
                Tags = tags,
                PageNumber = page ?? 1,
                TotalPages = (int)Math.Ceiling((decimal)pagedPostsResults.TotalPosts / NumPostsPerPage),
                ProfilePicUri = _settings.ProfilePicUri,
            });
        }

        public async Task<IActionResult> BlogPost(string route, CancellationTokenSource cts)
        {
            var postTask = _blogPostRepository.GetWithConditionAsync(x => x.Route.TrimStart('/') == route.TrimStart('/'), cts.Token);
            var featuredPostsTask = _blogPostRepository.GetFeaturedAsync(cts.Token);
            var recentPostsTask = _blogPostRepository.GetRecentAsync(NumRecentPosts, cts.Token);
            var tagsTask = _blogPostRepository.GetTagCountsAsync(cts.Token);

            var post = (await postTask).SingleOrDefault();
            if (post == null)
            {
                cts.Cancel();
                return NotFound();
            }

            var recent = await recentPostsTask;
            var featured = await featuredPostsTask;
            var tags = await tagsTask;

            return View(new PostViewModel
            {
                FeaturedPosts = featured,
                RecentPosts = recent,
                Post = post,
                DisqusDomainName = _settings.DisqusDomainName,
                Tags = tags,
                ProfilePicUri = _settings.ProfilePicUri,
            });
        }

        [Produces("application/xml")]
        public async Task<IActionResult> RssFeed(CancellationToken cancellationToken)
        {
            return Ok(await _feedGenerator.GenerateRssAsync(cancellationToken));
        }

        public IActionResult Error()
        {
            return View();
        }

        [Route("error/404")]
        public async Task<IActionResult> Error404(CancellationToken cancellationToken)
        {
            var featuredPostsTask = _blogPostRepository.GetFeaturedAsync(cancellationToken);
            var recentPostsTask = _blogPostRepository.GetRecentAsync(HomeController.NumRecentPosts, cancellationToken);
            var tagsTask = _blogPostRepository.GetTagCountsAsync(cancellationToken);

            var recent = await recentPostsTask;
            var featured = await featuredPostsTask;
            var tags = await tagsTask;

            return View(new BasicViewModel
                        {
                            FeaturedPosts = featured,
                            RecentPosts = recent,
                            Tags = tags,
                            ProfilePicUri = _settings.ProfilePicUri,
                        });
        }
    }
}
