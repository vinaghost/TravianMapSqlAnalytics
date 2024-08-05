using Features.Alliances;
using Features.Players;
using Features.Populations;
using Features.Populations.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Models.ViewModel.Alliances;

namespace WebMVC.Controllers
{
    public class AlliancesController(IMediator mediator) : Controller
    {
        private readonly IMediator _mediator = mediator;

        public async Task<IActionResult> Search(GetAlliancesByNameParameters parameters)
        {
            var alliances = await _mediator.Send(new GetAlliancesByNameQuery(parameters));
            return Json(new { results = alliances, pagination = new { more = alliances.PageNumber * alliances.PageSize < alliances.TotalItemCount } });
        }

        public IActionResult Index()
        {
            ViewBag.IsInput = false;
            return View();
        }

        public async Task<IActionResult> Index(int allianceId)
        {
            ViewBag.IsInput = true;
            var alliance = await _mediator.Send(new GetAllianceByIdQuery(allianceId));
            if (alliance is null) return View(new IndexViewModel { Alliance = alliance });

            var playerParameters = new GetPlayersParameters()
            {
                Alliances = [allianceId],
                PageSize = 60,
                PageNumber = 1
            };
            var players = await _mediator.Send(new GetPlayersQuery(playerParameters));

            if (players.Count <= 0)
            {
                return View(new IndexViewModel { Alliance = alliance });
            }

            var populationParameters = new PopulationParameters()
            {
                Ids = players.Select(p => p.PlayerId).ToList(),
                Days = 7,
            };
            var population = await _mediator.Send(new GetPlayersPopulationHistoryByIdQuery(populationParameters));
            return View(new IndexViewModel { Alliance = alliance, Players = [.. players], Population = population });
        }
    }
}