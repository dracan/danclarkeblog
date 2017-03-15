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

        public HomeController(IBlogPostRepository blogPostRepository, Settings _settings)
        {
            _blogPostRepository = blogPostRepository;
            this._settings = _settings;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            //(todo) This is getting much more than we need back. We don't need to blog content for all the posts - just the short version

            var posts = (await _blogPostRepository.GetAllAsync(cancellationToken)).ToList();

            return View(new HomeViewModel
            {
                FeaturedPosts = posts.Where(x => x.Featured).ToList(),
                Posts = posts,
            });
        }

        public async Task<IActionResult> BlogPost(string route, CancellationToken cancellationToken)
        {
            //(todo) This is getting much more than we need back. We don't need to blog content for all other posts

            var posts = (await _blogPostRepository.GetAllAsync(cancellationToken)).ToList();

            var post = posts.FirstOrDefault(x => x.Route.TrimStart('/') == route.TrimStart('/'));
            if (post == null)
            {
                return NotFound();
            }

            return View(new PostViewModel
            {
                FeaturedPosts = posts.Where(x => x.Featured).ToList(),
                Post = post,
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
