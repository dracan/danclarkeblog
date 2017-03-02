using System.Threading.Tasks;
using DanClarkeBlog.Core.Respositories;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index()
        {
            var posts = await _blogPostRepository.GetAllAsync();

            foreach(var post in posts)
            {
                _logger.Debug(post.HtmlText);
            }

            return View();
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
