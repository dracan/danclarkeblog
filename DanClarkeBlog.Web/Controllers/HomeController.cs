using System.Threading.Tasks;
using DanClarkeBlog.Core.Respositories;
using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepository;

        public HomeController(IBlogPostRepository blogPostRepository)
        {
            _blogPostRepository = blogPostRepository;
        }

        public async Task<IActionResult> Index()
        {
            await _blogPostRepository.GetAllAsync();

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
