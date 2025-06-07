using Features.Players.GetPlayerById;
using Features.Players.GetPlayersByName;
using Features.Populations;

using Features.Villages;
using Features.Villages.GetVillages;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Models.ViewModel.Players;

namespace WebMVC.Controllers
{
    public class PlayersController : Controller
    {
        public async Task<IResult> Names(
            GetPlayersByNameParameters parameters,
            [FromServices] GetPlayersByNameQuery.Handler getPlayersByNameQuery,
            [FromServices] IValidator<GetPlayersByNameParameters> validator)
        {
            var result = validator.Validate(parameters);
            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.ToDictionary());
            }

            var players = await getPlayersByNameQuery.HandleAsync(new(parameters));
            return Results.Json(new { results = players.Select(x => new { Id = x.PlayerId, Text = x.PlayerName }), pagination = new { more = players.PageNumber * players.PageSize < players.TotalItemCount } });
        }

        public async Task<IActionResult> Index(
            [FromServices] GetPlayerByIdQuery.Handler getPlayerByIdQuery,
            [FromServices] GetVillagesQuery.Handler getVillagesByParametersQuery,
            [FromServices] GetVillagesPopulationHistoryQuery.Handler getVillagesPopulationHistoryByParametersQuery,
            int playerId = -1
            )
        {
            if (playerId == -1)
            {
                ViewBag.IsInput = false;
                return View();
            }

            ViewBag.IsInput = true;
            var player = await getPlayerByIdQuery.HandleAsync(new(playerId));
            if (player is null) return View();

            var villageParameters = new GetVillagesParameters()
            {
                Players = [playerId],
                PageSize = 60,
                PageNumber = 1
            };
            var villages = await getVillagesByParametersQuery.HandleAsync(new(villageParameters));

            if (villages.Count <= 0)
            {
                return View(new IndexViewModel { Player = player });
            }

            var populationParameters = new PopulationParameters()
            {
                Ids = villages.Select(p => p.VillageId).ToList(),
                Days = 7,
            };
            var population = await getVillagesPopulationHistoryByParametersQuery.HandleAsync(new(populationParameters));
            return View(new IndexViewModel { Player = player, Villages = [.. villages], Population = population });
        }
    }
}