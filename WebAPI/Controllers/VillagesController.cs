using Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using WebAPI.Queries;
using WebAPI.Specifications.Villages;
using X.PagedList;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(404)]
    public class VillagesController(ServerDbContext dbContext, IMediator mediator) : ControllerBase
    {
        private readonly ServerDbContext _dbContext = dbContext;
        private readonly IMediator _mediator = mediator;

        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<Village>), 200)]
        public async Task<IActionResult> Get([FromBody] VillageParameters villageParameters)
        {
            var villageQuery = await _mediator.Send(new FilteredVillageQuery(villageParameters.Alliances, villageParameters.Players));

            var populationSpecification = new VillagePopulationSpecification()
            {
                Max = villageParameters.MaxPopulation,
                Min = villageParameters.MinPopulation,
            };

            villageQuery = populationSpecification.Apply(villageQuery);

            var rawVillages = await villageQuery
                .OrderByDescending(x => x.Population)
                .ToPagedListAsync(villageParameters.PageNumber, villageParameters.PageSize);

            var players = await _mediator.Send(new PlayerNameQuery(rawVillages.Select(x => x.PlayerId)));
            var alliances = await _mediator.Send(new AllianceInfoQuery(players.Values.Select(x => x.AllianceId)));

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
                        x.Name,
                        x.X,
                        x.Y,
                        x.Population,
                        x.IsCapital,
                        x.Tribe);
                });

            Response.Headers.Append("X-Pagination", rawVillages.ToXpagination().ToJson());
            return Ok(villages);
        }

        [HttpGet("change_population")]
        [ProducesResponseType(typeof(IPagedList<ChangePopulationVillage>), 200)]
        public async Task<IActionResult> Get([FromBody] ChangePopulationVillageParameters villageParameters)
        {
            var villageQuery = await _mediator.Send(new FilteredVillageQuery(villageParameters.Alliances, villageParameters.Players));
            var date = villageParameters.Date.ToDateTime(TimeOnly.MinValue);
            var rawVillages = await villageQuery
                .AsSplitQuery()
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
                    Populations = x.Populations.Select(x => new Population(x.Population, x.Date))
                })
                .OrderByDescending(x => x.ChangePopulation)
                .ToPagedListAsync(villageParameters.PageNumber, villageParameters.PageSize);

            var players = await _mediator.Send(new PlayerNameQuery(rawVillages.Select(x => x.PlayerId)));
            var alliances = await _mediator.Send(new AllianceInfoQuery(players.Values.Select(x => x.AllianceId)));

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

            Response.Headers.Append("X-Pagination", rawVillages.ToXpagination().ToJson());
            return Ok(villages);
        }
    }
}