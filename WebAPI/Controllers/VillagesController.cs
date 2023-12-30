using Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Extensions;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using WebAPI.Queries;
using X.PagedList;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(404)]
    public class VillagesController(ServerDbContext dbContext, IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly ServerDbContext _dbContext = dbContext;

        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<Village>), 200)]
        public async Task<IActionResult> Get([FromBody] VillageParameters villageParameters)
        {
            var (Alliances, Players, Villages, MinPopulation, MaxPopulation) = villageParameters;
            var villageCount = await _mediator.Send(new GetVillageCountQuery(Alliances, Players, Villages, MinPopulation, MaxPopulation));
            var rawVillages = await _dbContext.GetQueryable(villageParameters)
                .Select(x => new
                {
                    x.PlayerId,
                    x.VillageId,
                    VillageName = x.Name,
                    x.X,
                    x.Y,
                    x.Population,
                    x.IsCapital,
                    x.Tribe
                })
                .OrderByDescending(x => x.Population)
                .ToPagedListAsync(villageParameters.PageNumber, villageParameters.PageSize);

            var players = await _mediator.Send(new PlayerInfoQuery([.. rawVillages.Select(x => x.PlayerId)]));
            var alliances = await _mediator.Send(new AllianceInfoQuery([.. players.Values.Select(x => x.AllianceId)]));

            var villages = rawVillages
                .Select(x =>
                {
                    var player = players[x.PlayerId];
                    var alliance = alliances[player.AllianceId];
                    return new Village(
                        player.AllianceId,
                        alliance,
                        x.PlayerId,
                        player.Name,
                        x.VillageId,
                        x.VillageName,
                        x.X,
                        x.Y,
                        x.Population,
                        x.IsCapital,
                        x.Tribe);
                });

            Response.Headers.Append("X-Pagination", villages.ToXpagination().ToJson());
            return Ok(villages);
        }

        [HttpGet("change_population")]
        [ProducesResponseType(typeof(IPagedList<ChangePopulationVillage>), 200)]
        public async Task<IActionResult> Get([FromBody] ChangePopulationVillageParameters villageParameters)
        {
            var (Alliances, Players, Villages, MinPopulation, MaxPopulation) = villageParameters;
            var villageQuery = await _mediator.Send(new GetVillageCountQuery(Alliances, Players, Villages, MinPopulation, MaxPopulation));

            var date = villageParameters.Date.ToDateTime(TimeOnly.MinValue);
            var rawVillages = await _dbContext.GetQueryable(villageParameters)
                .Select(x => new
                {
                    x.PlayerId,
                    x.VillageId,
                    x.Name,
                    x.X,
                    x.Y,
                    x.IsCapital,
                    x.Tribe,
                    Populations = x.Populations.OrderByDescending(x => x.Date).Where(x => x.Date >= date),
                })
                .AsEnumerable()
                .Select(x => new
                {
                    x.PlayerId,
                    x.VillageId,
                    VillageName = x.Name,
                    x.X,
                    x.Y,
                    x.IsCapital,
                    x.Tribe,
                    ChangePopulation = x.Populations.Select(x => x.Population).FirstOrDefault() - x.Populations.Select(x => x.Population).LastOrDefault(),
                    Populations = x.Populations.Select(x => new PopulationRecord(x.Population, x.Date))
                })
                .Where(x => x.ChangePopulation >= villageParameters.MinChangePopulation)
                .Where(x => x.ChangePopulation <= villageParameters.MaxChangePopulation)
                .OrderByDescending(x => x.ChangePopulation)
                .ToPagedListAsync(villageParameters.PageNumber, villageParameters.PageSize);

            var players = await _mediator.Send(new PlayerInfoQuery([.. rawVillages.Select(x => x.PlayerId)]));
            var alliances = await _mediator.Send(new AllianceInfoQuery([.. players.Values.Select(x => x.AllianceId)]));

            var villages = rawVillages
                .Select(x =>
                {
                    var player = players[x.PlayerId];
                    var alliance = alliances[player.AllianceId];
                    return new ChangePopulationVillage(
                        player.AllianceId,
                        alliance,
                        x.PlayerId,
                        player.Name,
                        x.VillageId,
                        x.VillageName,
                        x.X,
                        x.Y,
                        x.IsCapital,
                        x.Tribe,
                        x.ChangePopulation,
                        x.Populations);
                });

            Response.Headers.Append("X-Pagination", villages.ToXpagination().ToJson());
            return Ok(villages);
        }
    }
}