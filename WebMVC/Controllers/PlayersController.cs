using Features.Players;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<IActionResult> Index(int playerId)
        {
            if (playerId == default) return View();
            var player = await _mediator.Send(new GetPlayerDataQuery(playerId));
            if (player is null) return View();
            return View(player);
        }

        public async Task<IActionResult> SearchAlliance(int allianceId)
        {
            var parameters = new GetPlayersParameters { AllianceId = allianceId };
            var players = await _mediator.Send(new GetPlayersByAllianceId(allianceId));
            return Json(new { results = players });
        }
    }
}