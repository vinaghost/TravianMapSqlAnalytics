using Core.Features.GetPlayerContainsAllianceHistory;
using Core.Features.GetPlayerContainsPopulation;
using Core.Features.GetPlayerContainsPopulationHistory;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class PlayersController : Controller
    {
        public IActionResult Index(PlayerContainsPopulationParameters parameters)
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