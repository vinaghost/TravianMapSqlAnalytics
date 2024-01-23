using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Disclaimer()
        {
            return View();
        }

        public IActionResult CookiePolicy()
        {
            return View();
        }
    }
}