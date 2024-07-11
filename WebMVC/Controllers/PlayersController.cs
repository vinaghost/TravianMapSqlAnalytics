using Features.GetPlayerData;
using Features.SearchPlayer;
using Features.Shared.Parameters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class PlayersController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        public async Task<IActionResult> Search(SearchParameters parameters)
        {
            var players = await _mediator.Send(new SearchPlayerByParametersQuery(parameters));
            return Json(new { results = players, pagination = new { more = players.PageNumber * players.PageSize < players.TotalItemCount } });
        }

        public async Task<IActionResult> Index(int playerId)
        {
            if (playerId == default) return View();
            var player = await _mediator.Send(new GetPlayerDataQuery(playerId));
            if (player is null) return View();
            return View(player);
        }
    }
}