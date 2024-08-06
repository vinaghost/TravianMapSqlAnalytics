using Features.Populations.Shared;
using Features.Shared.Dtos;
using Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Features.Populations
{
    public record GetVillagesPopulationHistoryByIdQuery(PopulationParameters Parameters) : ICachedQuery<Dictionary<int, List<PopulationDto>>>
    {
        public string CacheKey => $"{nameof(GetVillagesPopulationHistoryByIdQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetVillagesPopulationHistoryByIdQueryHandler(VillageDbContext dbContext) : IRequestHandler<GetVillagesPopulationHistoryByIdQuery, Dictionary<int, List<PopulationDto>>>
    {
        protected readonly VillageDbContext _dbContext = dbContext;

        public async Task<Dictionary<int, List<PopulationDto>>> Handle(GetVillagesPopulationHistoryByIdQuery request, CancellationToken cancellationToken)
        {
            var ids = request.Parameters.Ids;
            if (ids is null || ids.Count == 0)
            {
                return [];
            }

            var date = DateTime.Today.AddDays(-request.Parameters.Days);

            var population = await _dbContext.VillagesHistory
                .Where(x => ids.Distinct().Contains(x.VillageId))
                .Where(x => x.Date >= date)
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
                .AsParallel()
                .Select(x => new
                {
                    VillageId = x.Key,
                    Population = x.Select(x => new PopulationDto(x.Date, x.Population, x.ChangePopulation)).ToList(),
                })
                .ToDictionary(x => x.VillageId, x => x.Population);
        }
    }
}