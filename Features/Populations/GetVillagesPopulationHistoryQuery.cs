using System.Data;

using Dapper;

using Features.Shared.Constraints;

using Immediate.Handlers.Shared;

using Microsoft.Extensions.DependencyInjection;

namespace Features.Populations
{
    [Handler]
    public sealed partial class GetVillagesPopulationHistoryQuery([FromKeyedServices(DatabaseConnectionType.Villages)] IDbConnection connection)
    {
        public sealed record Query(PopulationParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetVillagesPopulationHistoryQuery)}_{Parameters.Key()}");

        private async ValueTask<Dictionary<int, List<PopulationDto>>> HandleAsync(Query query, CancellationToken cancellationToken)
        {
            var ids = query.Parameters.Ids;
            if (ids is null || ids.Count == 0)
            {
                return [];
            }

            var filterDate = DateTime.Today.AddDays(-query.Parameters.Days);

            const string sql = @"
                SELECT
                    VillageId,
                    Date,
                    Population,
                    ChangePopulation
                FROM VillagesHistory
                WHERE VillageId IN @Ids
                  AND Date >= @FilterDate
                ORDER BY Date DESC;";

            var rawHistory = await connection.QueryAsync<dynamic>(new(
                sql,
                new { Ids = ids, FilterDate = filterDate },
                cancellationToken: cancellationToken)
            );

            return rawHistory
                .GroupBy(x => (int)x.VillageId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(row => new PopulationDto(
                        (DateTime)row.Date,
                        (int)row.Population,
                        (int)row.ChangePopulation
                    )).ToList()
                );
        }
    }
}