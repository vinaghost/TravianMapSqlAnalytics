using Core.Parameters;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class PlayersController : Controller
    {
        public IActionResult Index(PlayerParameters parameters)
        {
            return View(parameters);
        }

        public IActionResult PopulationHistory(PlayerContainsPopulationHistoryParameters parameters)
        {
            return View(parameters);
        }

        public IActionResult AllianceHistory(PlayerContainsAllianceHistoryParameters parameters)
        {
            return View(parameters);
        }
    }
}