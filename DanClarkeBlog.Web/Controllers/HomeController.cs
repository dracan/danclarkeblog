using System;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Repositories;
using DanClarkeBlog.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading;
using Settings = DanClarkeBlog.Core.Settings;
using NLog;

namespace DanClarkeBlog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly Settings _settings;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private const int NumPostsPerPage = 10;
        private const int NumRecentPosts = 5;

        public HomeController(IBlogPostRepository blogPostRepository, Settings _settings)
        {
            _blogPostRepository = blogPostRepository;
            this._settings = _settings;
        }

        public async Task<IActionResult> Index(int? page, CancellationToken cancellationToken)
        {
            var offset = ((page ?? 1) - 1) * NumPostsPerPage;

            var pagedPostsTask = _blogPostRepository.GetAllAsync(offset, NumPostsPerPage, cancellationToken);
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
                TotalPages = (int)Math.Ceiling((decimal)pagedPostsResults.TotalPosts / NumPostsPerPage)
            });
        }

        public async Task<IActionResult> BlogPost(string route, CancellationToken cancellationToken)
        {
            var postTask = _blogPostRepository.GetWithConditionAsync(x => x.Route.TrimStart('/') == route.TrimStart('/'), cancellationToken);
            var featuredPostsTask = _blogPostRepository.GetFeaturedAsync(cancellationToken);
            var recentPostsTask = _blogPostRepository.GetRecentAsync(NumRecentPosts, cancellationToken);
            var tagsTask = _blogPostRepository.GetTagCountsAsync(cancellationToken);

            var post = (await postTask).SingleOrDefault();
            if (post == null)
            {
                //todo: Cancel the other tasks here?

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
                Tags = tags
            });
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
