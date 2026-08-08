using Microsoft.AspNetCore.Mvc;

namespace st10157545_abcretail.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
