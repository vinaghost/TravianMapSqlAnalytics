using Features.Populations.Shared;
using Features.Shared.Dtos;
using Features.Shared.Query;
using Microsoft.EntityFrameworkCore;

namespace Features.Populations
{
    public record GetVillagesPopulationHistoryByParametersQuery(PopulationParameters Parameters) : ICachedQuery<Dictionary<int, List<PopulationDto>>>
    {
        public string CacheKey => $"{nameof(GetVillagesPopulationHistoryByParametersQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetVillagesPopulationHistoryByParametersQueryHandler(VillageDbContext dbContext) : IRequestHandler<GetVillagesPopulationHistoryByParametersQuery, Dictionary<int, List<PopulationDto>>>
    {
        protected readonly VillageDbContext _dbContext = dbContext;

        public async Task<Dictionary<int, List<PopulationDto>>> Handle(GetVillagesPopulationHistoryByParametersQuery request, CancellationToken cancellationToken)
        {
            var ids = request.Parameters.Ids;
            if (ids is null || ids.Count == 0)
            {
                return [];
            }

            var predicate = PredicateBuilder.New<VillageHistory>(true);

            if (ids.Count == 1)
            {
                predicate = predicate.And(x => x.Id == ids[0]);
            }
            else
            {
                predicate = predicate.And(x => ids.Contains(x.Id));
            }

            var date = DateTime.Today.AddDays(-request.Parameters.Days);
            predicate = predicate.And(x => x.Date >= date);

            var population = await _dbContext.VillagesHistory
                .AsQueryable()
                .Where(predicate)
                .OrderByDescending(x => x.Date)
                .Select(x => new
                {
                    x.VillageId,
                    x.Date,
                    x.Population,
                    x.ChangePopulation,
                })
                .ToListAsync(cancellationToken);

            return population
                .GroupBy(x => x.VillageId)
                .Select(x => new
                {
                    VillageId = x.Key,
                    Population = x.Select(x => new PopulationDto(x.Date, x.Population, x.ChangePopulation)).ToList(),
                })
                .ToDictionary(x => x.VillageId, x => x.Population);
        }
    }
}