using Core.Features.GetPlayerContainsAllianceHistory;
using Core.Features.GetPlayerContainsPopulation;
using Core.Features.GetPlayerContainsPopulationHistory;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class PlayersController : Controller
    {
        public IActionResult Index(PlayerContainsPopulationParameters parameters)
        {
            return View(parameters);
        }

        public IActionResult PopulationHistory(PlayerContainsPopulationHistoryParameters parameters, [FromServices] IValidator<PlayerContainsPopulationHistoryParameters> validator)
        {
            var result = validator.Validate(parameters);
            if (!result.IsValid)
            {
                result.AddToModelState(ModelState);
            }
            return View(parameters);
        }

        public IActionResult AllianceHistory(PlayerContainsAllianceHistoryParameters parameters)
        {
            return View(parameters);
        }
    }
}