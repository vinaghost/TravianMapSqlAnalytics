using Core.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class VillagesController : Controller
    {
        public IActionResult Index(VillageContainsDistanceParameters parameters)
        {
            return View(parameters);
        }

        public IActionResult PopulationHistory(VillageContainsPopulationHistoryParameters parameters)
        {
            return View(parameters);
        }
    }
}