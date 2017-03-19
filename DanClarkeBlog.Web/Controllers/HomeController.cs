using System;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Respositories;
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

            var pagedResults = await _blogPostRepository.GetAllAsync(offset, NumPostsPerPage, cancellationToken);
            var featured = await _blogPostRepository.GetFeaturedAsync(cancellationToken);
            var recent = await _blogPostRepository.GetRecentAsync(NumRecentPosts, cancellationToken);

            return View(new HomeViewModel
            {
                FeaturedPosts = featured,
                RecentPosts = recent,
                Posts = pagedResults.Posts,
                PageNumber = page ?? 1,
                TotalPages = (int)Math.Ceiling((decimal)pagedResults.TotalPosts / NumPostsPerPage)
            });
        }

        public async Task<IActionResult> BlogPost(string route, CancellationToken cancellationToken)
        {
            var post = (await _blogPostRepository.GetWithConditionAsync(x => x.Route.TrimStart('/') == route.TrimStart('/'), cancellationToken)).SingleOrDefault();
            if (post == null)
            {
                return NotFound();
            }

            var recent = (await _blogPostRepository.GetRecentAsync(NumRecentPosts, cancellationToken)).ToList();
            var featured = await _blogPostRepository.GetFeaturedAsync(cancellationToken);

            return View(new PostViewModel
            {
                FeaturedPosts = featured,
                RecentPosts = recent,
                Post = post,
                DisqusDomainName = _settings.DisqusDomainName
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
