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
    }
}