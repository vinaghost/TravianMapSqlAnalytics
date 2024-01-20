using Core.Features.GetPlayerContainsAllianceHistory;
using Core.Features.GetPlayerContainsPopulation;
using Core.Features.GetPlayerContainsPopulationHistory;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Extensions;
using X.PagedList;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(404)]
    public class PlayersController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<PlayerContainsPopulationDto>), 200)]
        public async Task<IActionResult> Get([FromBody] PlayerContainsPopulationParameters playerParameters)
        {
            var players = await _mediator.Send(new GetPlayerContainsPopulationQuery(playerParameters));
            Response.Headers.Append("X-Pagination", players.ToJson());
            return Ok(players);
        }

        [HttpGet("population_history")]
        [ProducesResponseType(typeof(IPagedList<PlayerContainsPopulationHistory>), 200)]
        public async Task<IActionResult> Get([FromBody] PlayerContainsPopulationHistoryParameters playerParameters)
        {
            var players = await _mediator.Send(new GetPlayerContainsPopulationHistoryQuery(playerParameters));
            Response.Headers.Append("X-Pagination", players.ToJson());
            return Ok(players);
        }

        [HttpGet("alliance_history")]
        [ProducesResponseType(typeof(IPagedList<PlayerContainsAllianceHistory>), 200)]
        public async Task<IActionResult> Get([FromBody] PlayerContainsAllianceHistoryParameters playerParameters)
        {
            var players = await _mediator.Send(new GetPlayerContainsAllianceHistoryQuery(playerParameters));

            Response.Headers.Append("X-Pagination", players.ToJson());

            return Ok(players);
        }
    }
}