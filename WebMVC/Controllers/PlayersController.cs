using Features.Players;
using Features.Populations;
using Features.Populations.Shared;
using Features.Villages;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Models.ViewModel.Players;

namespace WebMVC.Controllers
{
    public class PlayersController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        public async Task<IActionResult> Search(GetPlayersByNameParameters parameters)
        {
            var players = await _mediator.Send(new GetPlayersByNameQuery(parameters));
            return Json(new { results = players, pagination = new { more = players.PageNumber * players.PageSize < players.TotalItemCount } });
        }

        public async Task<IActionResult> SearchAlliance(int allianceId)
        {
            var playerParameters = new GetPlayersParameters()
            {
                Alliances = [allianceId],
                PageSize = 60,
                PageNumber = 1
            };
            var players = await _mediator.Send(new GetPlayersQuery(playerParameters));
            return Json(new { results = players, pagination = new { more = players.PageNumber * players.PageSize < players.TotalItemCount } });
        }

        public IActionResult Index()
        {
            ViewBag.IsInput = false;
            return View();
        }

        public async Task<IActionResult> Index(int playerId)
        {
            ViewBag.IsInput = true;
            var player = await _mediator.Send(new GetPlayerByIdQuery(playerId));
            if (player is null) return View();

            var villageParameters = new GetVillagesParameters()
            {
                Players = [playerId],
                PageSize = 60,
                PageNumber = 1
            };
            var villages = await _mediator.Send(new GetVillagesQuery(villageParameters));

            if (villages.Count <= 0)
            {
                return View(new IndexViewModel { Player = player });
            }

            var populationParameters = new PopulationParameters()
            {
                Ids = villages.Select(p => p.PlayerId).ToList(),
                Days = 7,
            };
            var population = await _mediator.Send(new GetPlayersPopulationHistoryByIdQuery(populationParameters));
            return View(new IndexViewModel { Player = player, Villages = [.. villages], Population = population });
        }
    }
}