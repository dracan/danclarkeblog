using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Repositories;
using DanClarkeBlog.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading;
using DanClarkeBlog.Core.Helpers;
using Microsoft.Extensions.Options;
using Settings = DanClarkeBlog.Core.Settings;

namespace DanClarkeBlog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly Settings _settings;
        private readonly IFeedGenerator _feedGenerator;
        private readonly ISearchHelper _searchHelper;
        private const int NumPostsPerPage = 10;
        private const int NumRecentPosts = 5;

        private readonly Dictionary<string, Guid> _specialPages = new Dictionary<string, Guid>
        {
            { "about", new Guid("a760d5f4-a372-4de4-a527-a578eeb09e82") },
            { "public-speaking", new Guid("07d4b359-3797-4acd-8173-4a62dcd995e8") },
        };

        public HomeController(IBlogPostRepository blogPostRepository, IOptions<Settings> settings, IFeedGenerator feedGenerator, ISearchHelper searchHelper)
        {
            _blogPostRepository = blogPostRepository;
            _settings = settings.Value;
            _feedGenerator = feedGenerator;
            _searchHelper = searchHelper;
        }

        public async Task<IActionResult> Search([FromQuery] string term, [FromQuery] int? page, CancellationToken cancellationToken)
        {
            var offset = ((page ?? 1) - 1) * NumPostsPerPage;

            var pagedPostsTask = _searchHelper.SearchAsync(term, offset, NumPostsPerPage);
            var featuredPostsTask = _blogPostRepository.GetFeaturedAsync(cancellationToken);
            var recentPostsTask = _blogPostRepository.GetRecentAsync(NumRecentPosts, cancellationToken);
            var tagsTask = _blogPostRepository.GetTagCountsAsync(cancellationToken);

            var pagedPostsResults = await pagedPostsTask;
            var featuredPosts = await featuredPostsTask;
            var recentPosts = await recentPostsTask;
            var tags = await tagsTask;

            ViewData["Title"] = term;

            return View("Index", new HomeViewModel
            {
                FeaturedPosts = featuredPosts,
                RecentPosts = recentPosts,
                Posts = pagedPostsResults?.Posts,
                Tags = tags,
                PageNumber = page ?? 1,
                TotalPages = (int)Math.Ceiling((decimal)(pagedPostsResults?.TotalPosts ?? 0) / NumPostsPerPage),
                ProfilePicUri = _settings.ProfilePicUri,
                GoogleTagId = _settings.GoogleTagId,
                VersionNumber = _settings.VersionNumber,
            });
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

            if (!string.IsNullOrWhiteSpace(tag) && pagedPostsResults.TotalPosts == 0) // 404 on a tag listing page with an invalid tag
                return NotFound();

            // Use the tag name from the database, not from the query string to preserve case
            ViewData["Title"] = string.IsNullOrWhiteSpace(tag)
                ? "" : pagedPostsResults?.Posts?.FirstOrDefault()?.BlogPostTags?.SingleOrDefault(x => string.Equals(x.TagName, tag, StringComparison.CurrentCultureIgnoreCase))?.TagName ?? "";

            return View(new HomeViewModel
            {
                FeaturedPosts = featuredPosts,
                RecentPosts = recentPosts,
                Posts = pagedPostsResults?.Posts,
                Tags = tags,
                PageNumber = page ?? 1,
                TotalPages = (int)Math.Ceiling((decimal)(pagedPostsResults?.TotalPosts ?? 0) / NumPostsPerPage),
                ProfilePicUri = _settings.ProfilePicUri,
                GoogleTagId = _settings.GoogleTagId,
                VersionNumber = _settings.VersionNumber,
            });
        }

        public async Task<IActionResult> BlogPost(string route, CancellationTokenSource cts)
        {
            var postTask = _specialPages.TryGetValue(route, out var specialPageId)
                ? _blogPostRepository.GetDraftByIdAsync(specialPageId, cts.Token)
                : Guid.TryParse(route, out var draftId)
                    ? _blogPostRepository.GetDraftByIdAsync(draftId, cts.Token) // For draft, use ID as route
                    : _blogPostRepository.GetPublishedByRouteAsync(route, cts.Token); // For published, match Route property in blog post

            var featuredPostsTask = _blogPostRepository.GetFeaturedAsync(cts.Token);
            var recentPostsTask = _blogPostRepository.GetRecentAsync(NumRecentPosts, cts.Token);
            var tagsTask = _blogPostRepository.GetTagCountsAsync(cts.Token);

            var post = await postTask;
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
                GoogleTagId = _settings.GoogleTagId,
                VersionNumber = _settings.VersionNumber,
            });
        }

        [Produces("application/xml")]
        public async Task<IActionResult> RssFeed(CancellationToken cancellationToken)
        {
            var xml = await _feedGenerator.GenerateRssAsync(cancellationToken);
            return Content(xml, "application/xml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        [Route("error/404")]
        public async Task<IActionResult> Error404(CancellationToken cancellationToken)
        {
            var featuredPostsTask = _blogPostRepository.GetFeaturedAsync(cancellationToken);
            var recentPostsTask = _blogPostRepository.GetRecentAsync(NumRecentPosts, cancellationToken);
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
                            GoogleTagId = _settings.GoogleTagId,
                            VersionNumber = _settings.VersionNumber,
                        });
        }
    }
}
