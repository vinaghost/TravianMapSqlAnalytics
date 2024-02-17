using Core.Features.GetPlayerContainsAllianceHistory;
using Core.Features.GetPlayerContainsPopulation;
using Core.Features.GetPlayerContainsPopulationHistory;
using Core.Features.SearchPlayer;
using Core.Features.Shared.Parameters;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class PlayersController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        public IActionResult Index(PlayerContainsPopulationParameters parameters, [FromServices] IValidator<PlayerContainsPopulationParameters> validator)
        {
            var result = validator.Validate(parameters);
            result.AddToModelState(ModelState);
            return View(parameters);
        }

        public IActionResult PopulationHistory(PlayerContainsPopulationHistoryParameters parameters, [FromServices] IValidator<PlayerContainsPopulationHistoryParameters> validator)
        {
            var result = validator.Validate(parameters);
            result.AddToModelState(ModelState);
            return View(parameters);
        }

        public IActionResult InactivePlayers(PlayerContainsPopulationHistoryParameters parameters, [FromServices] IValidator<PlayerContainsPopulationHistoryParameters> validator)
        {
            var result = validator.Validate(parameters);
            result.AddToModelState(ModelState);
            return View(parameters);
        }

        public IActionResult AllianceHistory(PlayerContainsAllianceHistoryParameters parameters, [FromServices] IValidator<PlayerContainsAllianceHistoryParameters> validator)
        {
            var result = validator.Validate(parameters);
            result.AddToModelState(ModelState);
            return View(parameters);
        }

        public async Task<IActionResult> Search(SearchParameters parameters)
        {
            var players = await _mediator.Send(new SearchPlayerByParametersQuery(parameters));
            return Json(new { results = players, pagination = new { more = (players.PageNumber * players.Count) < players.TotalItemCount } });
        }
    }
}