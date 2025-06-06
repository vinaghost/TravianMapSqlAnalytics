using Features.Alliances.GetAllianceById;
using Features.Alliances.GetAlliancesByName;
using Features.Players.GetPlayers;
using Features.Populations;

using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Models.ViewModel.Alliances;

namespace WebMVC.Controllers
{
    public class AlliancesController : Controller
    {
        public async Task<IResult> Names(
            GetAlliancesByNameParameters parameters,
            [FromServices] GetAlliancesByNameQuery.Handler getAlliancesByNameQuery,
            [FromServices] IValidator<GetAlliancesByNameParameters> validator)
        {
            var result = validator.Validate(parameters);
            if (!result.IsValid)
            {
                return Results.ValidationProblem(result.ToDictionary());
            }
            var alliances = await getAlliancesByNameQuery.HandleAsync(new(parameters));
            return Results.Json(new { results = alliances.Select(x => new { Id = x.AllianceId, Text = x.AllianceName }), pagination = new { more = alliances.PageNumber * alliances.PageSize < alliances.TotalItemCount } });
        }

        public async Task<IActionResult> Index(
            [FromServices] GetAllianceByIdQuery.Handler getAllianceByIdQuery,
            [FromServices] GetPlayersQuery.Handler getPlayersByParametersQuery,
            [FromServices] GetPlayersPopulationHistoryQuery.Handler getPlayersPopulationHistoryByParametersQuery,
            int allianceId = -1)
        {
            if (allianceId == -1)
            {
                ViewBag.IsInput = false;
                return View();
            }
            ViewBag.IsInput = true;
            var alliance = await getAllianceByIdQuery.HandleAsync(new(allianceId));
            if (alliance is null) return View(new IndexViewModel { Alliance = alliance });

            var playerParameters = new GetPlayersParameters()
            {
                Alliances = [allianceId],
                PageSize = 60,
                PageNumber = 1
            };
            var players = await getPlayersByParametersQuery.HandleAsync(new(playerParameters));

            if (players.Count <= 0)
            {
                return View(new IndexViewModel { Alliance = alliance });
            }

            var populationParameters = new PopulationParameters()
            {
                Ids = players.Select(p => p.PlayerId).ToList(),
                Days = 7,
            };
            var population = await getPlayersPopulationHistoryByParametersQuery.HandleAsync(new(populationParameters));
            return View(new IndexViewModel { Alliance = alliance, Players = players.ToList(), Population = population });
        }
    }
}