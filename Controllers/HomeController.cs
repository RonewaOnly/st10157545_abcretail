using Microsoft.AspNetCore.Mvc;
using st10157545_abcretail.Models;
using System.Diagnostics;

namespace st10157545_abcretail.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View();
        }

    }
}
