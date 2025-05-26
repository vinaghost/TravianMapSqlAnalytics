using Features.Constraints;
using Features.Dtos;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Features.Queries.Populations
{
    [Handler]
    public static partial class GetVillagesPopulationHistoryQuery
    {
        public sealed record Query(PopulationParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetVillagesPopulationHistoryQuery)}_{Parameters.Key()}");

        private static async ValueTask<Dictionary<int, List<PopulationDto>>> HandleAsync(
            Query query,
            VillageDbContext context,
            CancellationToken cancellationToken
        )
        {
            var ids = query.Parameters.Ids;
            if (ids is null || ids.Count == 0)
            {
                return [];
            }

            var predicate = PredicateBuilder.New<VillageHistory>(true);

            if (ids.Count == 1)
            {
                predicate = predicate.And(x => x.VillageId == ids[0]);
            }
            else
            {
                predicate = predicate.And(x => ids.Contains(x.VillageId));
            }

            var date = DateTime.Today.AddDays(-query.Parameters.Days);
            predicate = predicate.And(x => x.Date >= date);

            var population = await context.VillagesHistory
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