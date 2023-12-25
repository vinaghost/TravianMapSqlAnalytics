using Core;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using X.PagedList;

using PlayerEnitty = Core.Models.Player;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayersController(ServerDbContext dbContext) : ControllerBase
    {
        private readonly ServerDbContext _dbContext = dbContext;

        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<Player>), 200)]
        public async Task<IActionResult> Get([FromBody] PlayerParameters playerParameters)
        {
            IQueryable<PlayerEnitty> query;
            if (playerParameters.Alliances.Count > 0)
            {
                query = _dbContext.Alliances
                    .Where(x => playerParameters.Alliances.Contains(x.AllianceId))
                    .SelectMany(x => x.Players);
            }
            else
            {
                query = _dbContext.Players
                   .AsQueryable();
            }

            var players = await query
                .OrderByDescending(x => x.Villages.Count())
                .Select(x => new Player(x.PlayerId, x.Name))
                .ToPagedListAsync(playerParameters.PageNumber, playerParameters.PageSize);

            Response.Headers.Append("X-Pagination", players.ToXpagination().ToJson());

            return Ok(players);
        }
    }
}