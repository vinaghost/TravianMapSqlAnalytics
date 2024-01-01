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

        [HttpGet("change_population")]
        [ProducesResponseType(typeof(IPagedList<PlayerHasChangePopulation>), 200)]
        public async Task<IActionResult> Get([FromBody] PlayerHasChangePopulationParameters playerParameters)
        {
            var players = await _mediator.Send(new GetChangePopulationPlayersQuery(playerParameters));
            Response.Headers.Append("X-Pagination", players.ToXpagination().ToJson());
            return Ok(players);
        }

        [HttpGet("change_alliance")]
        [ProducesResponseType(typeof(IPagedList<Player>), 200)]
        public async Task<IActionResult> Get([FromBody] PlayerHasChangeAllianceParameters playerParameters)
        {
            var players = await _mediator.Send(new GetChangeAlliancePlayersQuery(playerParameters));

            Response.Headers.Append("X-Pagination", players.ToXpagination().ToJson());

            return Ok(players);
        }
    }
}