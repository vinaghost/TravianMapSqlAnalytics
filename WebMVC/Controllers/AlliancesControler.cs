using Core.Features.SearchAlliance;
using Core.Features.Shared.Models;
using Core.Features.Shared.Parameters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebMVC.Controllers
{
    public class AlliancesControler(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        public async Task<IActionResult> AllianceList(SearchParameters parameters)
        {
            var alliances = await _mediator.Send<IList<SearchResult>>(new GetAllianceQuery(parameters));
            var result = alliances
                .Take(20)
                .ToList();
            return Json(new { items = result });
        }
    }
}