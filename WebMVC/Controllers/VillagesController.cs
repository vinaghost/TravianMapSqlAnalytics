using Core.Features.GetVillageContainsDistance;
using Core.Features.GetVillageContainsPopulationHistory;
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