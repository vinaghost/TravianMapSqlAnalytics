using Core;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using WebAPI.Specifications.Players;
using X.PagedList;

using PlayerEnitty = Core.Models.Player;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(404)]
    public class PlayersController(ServerDbContext dbContext) : ControllerBase
    {
        private readonly ServerDbContext _dbContext = dbContext;

        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<Player>), 200)]
        public async Task<IActionResult> Get([FromBody] PlayerParameters playerParameters)
        {
            var query = GetBaseQuery(playerParameters);

            var players = await query
                .OrderByDescending(x => x.Villages.Count())
                .Select(x => new Player(x.PlayerId, x.Name, x.Villages.Count(), x.Villages.Select(x => x.Population).Sum()))
                .ToPagedListAsync(playerParameters.PageNumber, playerParameters.PageSize);

            Response.Headers.Append("X-Pagination", players.ToXpagination().ToJson());

            return Ok(players);
        }

        [HttpGet("change_population")]
        [ProducesResponseType(typeof(IPagedList<Player>), 200)]
        public async Task<IActionResult> Get([FromBody] ChangePopulationPlayerParameters playerParameters)
        {
            var query = GetBaseQuery(playerParameters);

            var players = await query
                .OrderByDescending(x => x.Villages.Count())
                .Select(x => new Player(x.PlayerId, x.Name, x.Villages.Count(), x.Villages.Select(x => x.Population).Sum()))
                .ToPagedListAsync(playerParameters.PageNumber, playerParameters.PageSize);

            Response.Headers.Append("X-Pagination", players.ToXpagination().ToJson());

            return Ok(players);
        }

        private IQueryable<PlayerEnitty> GetBaseQuery(PlayerParameters playerParameters)
        {
            IQueryable<PlayerEnitty> query;
            if (playerParameters.Alliances.Count > 0)
            {
                var specification = new PlayerFilterSpecification() { Ids = playerParameters.Alliances };
                query = specification.Apply(_dbContext.Alliances.AsQueryable());
            }
            else
            {
                query = _dbContext.Players.AsQueryable();
            }
            return query;
        }
    }
}