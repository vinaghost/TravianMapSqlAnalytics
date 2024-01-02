using Core;
using Core.Models;
using Core.Parameters;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Extensions;
using X.PagedList;

using AllianceEnitty = Core.Entities.Alliance;

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

            Response.Headers.Append("X-Pagination", alliances.ToJson());

            return Ok(alliances);
        }
    }
}