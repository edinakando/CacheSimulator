using Microsoft.AspNetCore.Mvc;

namespace CacheSimulator.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}