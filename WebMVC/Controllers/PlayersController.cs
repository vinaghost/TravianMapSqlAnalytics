using Features.Players;
using Features.Populations;
using Features.Populations.Shared;
using Features.Villages.ByDistance;
using Features.Villages.Shared;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Models.ViewModel.Players;

namespace WebMVC.Controllers
{
    public class PlayersController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        public async Task<IResult> Names(GetPlayersByNameParameters parameters, [FromServices] IValidator<GetPlayersByNameParameters> validator)
        {
            var result = validator.Validate(parameters);
            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.ToDictionary());
            }

            var players = await _mediator.Send(new GetPlayersByNameQuery(parameters));
            return Results.Json(new { results = players.Select(x => new { Id = x.PlayerId, Text = x.PlayerName }), pagination = new { more = players.PageNumber * players.PageSize < players.TotalItemCount } });
        }

        public async Task<IActionResult> Index(int playerId = -1)
        {
            if (playerId == -1)
            {
                ViewBag.IsInput = false;
                return View();
            }

            ViewBag.IsInput = true;
            var player = await _mediator.Send(new GetPlayerByIdQuery(playerId));
            if (player is null) return View();

            var villageParameters = new VillagesParameters()
            {
                Players = [playerId],
                PageSize = 60,
                PageNumber = 1
            };
            var villages = await _mediator.Send(new GetVillagesByParameters(villageParameters));

            if (villages.Count <= 0)
            {
                return View(new IndexViewModel { Player = player });
            }

            var populationParameters = new PopulationParameters()
            {
                Ids = villages.Select(p => p.VillageId).ToList(),
                Days = 7,
            };
            var population = await _mediator.Send(new GetVillagesPopulationHistoryByParametersQuery(populationParameters));
            return View(new IndexViewModel { Player = player, Villages = [.. villages], Population = population });
        }
    }
}