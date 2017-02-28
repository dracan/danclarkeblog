using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DanClarkeBlog.Core.Respositories;
using Microsoft.AspNetCore.Mvc;

namespace DanClarkeBlog.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepository;

        public HomeController(IBlogPostRepository blogPostRepository)
        {
            _blogPostRepository = blogPostRepository;
        }

        public IActionResult Index()
        {
            _blogPostRepository.GetAll();

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
