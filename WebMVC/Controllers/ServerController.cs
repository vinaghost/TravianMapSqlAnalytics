using Core.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class ServersController : Controller
    {
        public IActionResult Index(ServerParameters parameters)
        {
            return View(parameters);
        }
    }
}