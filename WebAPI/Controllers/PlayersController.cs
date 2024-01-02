using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using WebAPI.Queries;
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
        [ProducesResponseType(typeof(IPagedList<Player>), 200)]
        public async Task<IActionResult> Get([FromBody] PlayerParameters playerParameters)
        {
            var players = await _mediator.Send(new GetPlayersQuery(playerParameters));
            Response.Headers.Append("X-Pagination", players.ToXpagination().ToJson());
            return Ok(players);
        }

        [HttpGet("population_history")]
        [ProducesResponseType(typeof(IPagedList<PlayerContainsPopulationHistory>), 200)]
        public async Task<IActionResult> Get([FromBody] PlayerContainsPopulationHistoryParameters playerParameters)
        {
            var players = await _mediator.Send(new GetChangePopulationPlayersQuery(playerParameters));
            Response.Headers.Append("X-Pagination", players.ToXpagination().ToJson());
            return Ok(players);
        }

        [HttpGet("alliance_history")]
        [ProducesResponseType(typeof(IPagedList<PlayerContainsAllianceHistory>), 200)]
        public async Task<IActionResult> Get([FromBody] PlayerContainsAllianceHistoryParameters playerParameters)
        {
            var players = await _mediator.Send(new GetChangeAlliancePlayersQuery(playerParameters));

            Response.Headers.Append("X-Pagination", players.ToXpagination().ToJson());

            return Ok(players);
        }
    }
}