using Features.Alliances;
using Features.Players;
using Features.Populations;
using Features.Populations.Shared;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Models.ViewModel.Alliances;

namespace WebMVC.Controllers
{
    public class AlliancesController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        public async Task<IResult> Names(GetAlliancesByNameParameters parameters, [FromServices] IValidator<GetAlliancesByNameParameters> validator)
        {
            var result = validator.Validate(parameters);
            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.ToDictionary());
            }
            var alliances = await _mediator.Send(new GetAlliancesByNameQuery(parameters));
            return Results.Json(new { results = alliances.Select(x => new { Id = x.AllianceId, Text = x.AllianceName }), pagination = new { more = alliances.PageNumber * alliances.PageSize < alliances.TotalItemCount } });
        }

        public async Task<IActionResult> Index(int allianceId = -1)
        {
            if (allianceId == -1)
            {
                ViewBag.IsInput = false;
                return View();
            }
            ViewBag.IsInput = true;
            var alliance = await _mediator.Send(new GetAllianceByIdQuery(allianceId));
            if (alliance is null) return View(new IndexViewModel { Alliance = alliance });

            var playerParameters = new PlayersParameters()
            {
                Alliances = [allianceId],
                PageSize = 60,
                PageNumber = 1
            };
            var players = await _mediator.Send(new GetPlayersByParametersQuery(playerParameters));

            if (players.Count <= 0)
            {
                return View(new IndexViewModel { Alliance = alliance });
            }

            var populationParameters = new PopulationParameters()
            {
                Ids = players.Select(p => p.PlayerId).ToList(),
                Days = 7,
            };
            var population = await _mediator.Send(new GetPlayersPopulationHistoryByParametersQuery(populationParameters));
            return View(new IndexViewModel { Alliance = alliance, Players = [.. players], Population = population });
        }
    }
}