using Features.SearchAlliance;
using Features.SearchPlayer;
using Features.Shared.Parameters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class AlliancesController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        public async Task<IActionResult> Search(SearchParameters parameters)
        {
            var alliances = await _mediator.Send(new SearchAllianceByParametersQuery(parameters));
            return Json(new { results = alliances, pagination = new { more = alliances.PageNumber * alliances.PageSize < alliances.TotalItemCount } });
        }

        public async Task<IActionResult> SearchPlayer(int allianceId)
        {
            var players = await _mediator.Send(new SearchPlayerByAllianceIdQuery(allianceId));
            return Json(new { results = players });
        }
    }
}