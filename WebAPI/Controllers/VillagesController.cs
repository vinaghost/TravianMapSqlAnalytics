using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using WebAPI.Specifications;
using X.PagedList;
using VillageEnitty = Core.Models.Village;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ProducesResponseType(404)]
    public class VillagesController(ServerDbContext dbContext) : ControllerBase
    {
        private readonly ServerDbContext _dbContext = dbContext;

        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<Village>), 200)]
        public async Task<IActionResult> Get([FromBody] VillageParameters villageParameters)
        {
            var baseQuery = GetBaseQuery(villageParameters);

            var villages = await baseQuery
                .OrderByDescending(x => x.Population)
                .Select(x => new Village(x.VillageId,
                                         x.Name,
                                         x.X,
                                         x.Y,
                                         x.Population,
                                         x.IsCapital))
                .ToPagedListAsync(villageParameters.PageNumber, villageParameters.PageSize);

            Response.Headers.Append("X-Pagination", villages.ToXpagination().ToJson());
            return Ok(villages);
        }

        [HttpGet("change_population")]
        [ProducesResponseType(typeof(IPagedList<ChangePopulationVillage>), 200)]
        public async Task<IActionResult> Get([FromBody] ChangePopulationVillageParameters villageParameters)
        {
            var newestDate = await _dbContext.VillagesPopulations
                .OrderByDescending(x => x.Date)
                .Select(x => x.Date)
                .FirstOrDefaultAsync();

            var minDate = newestDate.AddDays(-villageParameters.Days);

            var baseQuery = GetBaseQuery(villageParameters);

            var populationQuery = baseQuery
                .Join(
                    _dbContext.VillagesPopulations
                        .Where(x => x.Date <= newestDate || x.Date >= minDate),
                    x => x.VillageId,
                    x => x.VillageId,
                    (village, population) => new
                    {
                        village.VillageId,
                        VillageName = village.Name,
                        village.X,
                        village.Y,
                        village.IsCapital,
                        population.Population,
                        population.Date,
                    })
                .GroupBy(x => x.VillageId)
                .AsEnumerable()
                .Select(x =>
                {
                    var village = x.First();
                    var populations = x.OrderByDescending(x => x.Date).Select(x => new Population(x.Population, x.Date)).ToList();
                    return new ChangePopulationVillage(
                        x.Key,
                        village.VillageName,
                        village.X,
                        village.Y,
                        village.IsCapital,
                        populations.First().Amount - populations.Last().Amount,
                        populations);
                });

            var villages = await populationQuery
                .OrderByDescending(x => x.ChangePopulation)
                .ToPagedListAsync(villageParameters.PageNumber, villageParameters.PageSize);

            Response.Headers.Append("X-Pagination", villages.ToXpagination().ToJson());
            return Ok(villages);
        }

        private IQueryable<VillageEnitty> GetBaseQuery(VillageParameters villageParameters)
        {
            IQueryable<VillageEnitty> query;
            if (villageParameters.Alliances.Count > 0)
            {
                var specification = new VillageFilterSpecification() { Ids = villageParameters.Alliances };
                query = specification.Apply(_dbContext.Alliances.AsQueryable());
            }
            else if (villageParameters.Players.Count > 0)
            {
                var specification = new VillageFilterSpecification() { Ids = villageParameters.Players };
                query = specification.Apply(_dbContext.Players.AsQueryable());
            }
            else
            {
                query = _dbContext.Villages.AsQueryable();
            }

            var populationSpecification = new VillagePopulationSpecification() { Min = villageParameters.MinPopulation, Max = villageParameters.MaxPopulation };
            query = populationSpecification.Apply(query);
            return query;
        }
    }
}