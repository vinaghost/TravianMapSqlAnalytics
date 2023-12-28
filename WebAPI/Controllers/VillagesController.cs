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

            var villages = await villageQuery
                .OrderByDescending(x => x.Population)
                .ToPagedListAsync(villageParameters.PageNumber, villageParameters.PageSize);

            Response.Headers.Append("X-Pagination", villages.ToXpagination().ToJson());
            return Ok(villages);
        }

        [HttpGet("change_population")]
        [ProducesResponseType(typeof(IPagedList<ChangePopulationVillage>), 200)]
        public async Task<IActionResult> Get([FromBody] ChangePopulationVillageParameters villageParameters)
        {
            var villageQuery = await _mediator.Send(new FilteredVillageQuery(villageParameters.Alliances, villageParameters.Players));
            var date = villageParameters.Date.ToDateTime(TimeOnly.MinValue);
            var populationQuery = villageQuery
                .AsSplitQuery()
                .Select(x => new
                {
                    x.VillageId,
                    x.Name,
                    x.X,
                    x.Y,
                    x.IsCapital,
                    Populations = x.Populations.OrderByDescending(x => x.Date).Where(x => x.Date >= date),
                })
                .AsEnumerable()
                .Select(x => new ChangePopulationVillage(
                        x.VillageId,
                        x.Name,
                        x.X,
                        x.Y,
                        x.IsCapital,
                        x.Populations.Select(x => x.Population).FirstOrDefault() - x.Populations.Select(x => x.Population).LastOrDefault(),
                        x.Populations.Select(x => new Population(x.Population, x.Date))));

            var villages = await populationQuery
                .OrderByDescending(x => x.ChangePopulation)
                .ToPagedListAsync(villageParameters.PageNumber, villageParameters.PageSize);

            Response.Headers.Append("X-Pagination", villages.ToXpagination().ToJson());
            return Ok(villages);
        }
    }
}