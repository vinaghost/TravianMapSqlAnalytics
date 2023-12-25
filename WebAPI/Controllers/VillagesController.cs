using Core;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using X.PagedList;

using VillageEnitty = Core.Models.Village;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VillagesController(ServerDbContext dbContext) : ControllerBase
    {
        private readonly ServerDbContext _dbContext = dbContext;

        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<Village>), 200)]
        public async Task<IActionResult> Get([FromBody] VillageParameters villageParameters)
        {
            IQueryable<VillageEnitty> query;
            if (villageParameters.Alliances.Count > 0)
            {
                query = _dbContext.Alliances
                    .Where(x => villageParameters.Alliances.Contains(x.AllianceId))
                    .SelectMany(x => x.Players)
                    .SelectMany(x => x.Villages);
            }
            else if (villageParameters.Players.Count > 0)
            {
                query = _dbContext.Players
                    .Where(x => villageParameters.Players.Contains(x.PlayerId))
                    .SelectMany(x => x.Villages);
            }
            else
            {
                query = _dbContext.Villages
                   .AsQueryable();
            }

            query = query
                .Where(x => x.Population >= villageParameters.MinPopulation)
                .Where(x => x.Population <= villageParameters.MaxPopulation)
                .OrderByDescending(x => x.Population);

            var villages = await query
                .Select(x => new Village(x.VillageId, x.Name, x.X, x.Y, x.Population))
                .ToPagedListAsync(villageParameters.PageNumber, villageParameters.PageSize);

            Response.Headers.Append("X-Pagination", villages.ToXpagination().ToJson());
            return Ok(villages);
        }
    }
}