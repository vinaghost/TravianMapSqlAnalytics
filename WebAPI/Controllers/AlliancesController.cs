using Core;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using X.PagedList;

using AllianceEnitty = Core.Models.Alliance;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlliancesController(ServerDbContext dbContext) : ControllerBase
    {
        private readonly ServerDbContext _dbContext = dbContext;

        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<Alliance>), 200)]
        public async Task<IActionResult> Get([FromBody] AlliancesParameters allianceParameters)
        {
            IQueryable<AllianceEnitty> query;

            query = _dbContext.Alliances
                .AsQueryable();

            var alliances = await query
                .OrderByDescending(x => x.Players.Count())
                .Select(x => new Alliance(x.AllianceId, x.Name))
                .ToPagedListAsync(allianceParameters.PageNumber, allianceParameters.PageSize);

            Response.Headers.Append("X-Pagination", alliances.ToXpagination().ToJson());

            return Ok(alliances);
        }
    }
}