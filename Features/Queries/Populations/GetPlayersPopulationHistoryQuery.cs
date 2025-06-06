using Features.Dtos;
using Features.Shared.Constraints;
using Immediate.Handlers.Shared;
using Infrastructure.DbContexts;
using Infrastructure.Entities;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Features.Queries.Populations
{
    [Handler]
    public static partial class GetPlayersPopulationHistoryQuery
    {
        public sealed record Query(PopulationParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetPlayersPopulationHistoryQuery)}_{Parameters.Key()}");

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

            var predicate = PredicateBuilder.New<PlayerHistory>(true);

            if (ids.Count == 1)
            {
                predicate = predicate.And(x => x.PlayerId == ids[0]);
            }
            else
            {
                predicate = predicate.And(x => ids.Contains(x.PlayerId));
            }

            var date = DateTime.Today.AddDays(-query.Parameters.Days);
            predicate = predicate.And(x => x.Date >= date);

            var population = await context.PlayersHistory
                .AsQueryable()
                .Where(predicate)
                .OrderByDescending(x => x.Date)
                .Select(x => new
                {
                    x.PlayerId,
                    x.Date,
                    x.Population,
                    x.ChangePopulation,
                })
                .ToListAsync(cancellationToken);

            return population
                .GroupBy(x => x.PlayerId)
                .Select(x => new
                {
                    PlayerId = x.Key,
                    Population = x.Select(x => new PopulationDto(x.Date, x.Population, x.ChangePopulation)).ToList(),
                })
                .ToDictionary(x => x.PlayerId, x => x.Population);
        }
    }
}