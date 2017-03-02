using System.Threading.Tasks;
using DanClarkeBlog.Core.Respositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
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

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> BlogPost(string route)
        {
            var posts = await _blogPostRepository.GetAllAsync();

            var post = posts.FirstOrDefault(x => x.Route.TrimStart('/') == route.TrimStart('/'));
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
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
