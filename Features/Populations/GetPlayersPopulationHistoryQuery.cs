using System.Data;

using Dapper;

using Features.Shared.Constraints;

using Immediate.Handlers.Shared;

using Microsoft.Extensions.DependencyInjection;

namespace Features.Populations
{
    [Handler]
    public sealed partial class GetPlayersPopulationHistoryQuery([FromKeyedServices(DatabaseConnectionType.Villages)] IDbConnection connection)
    {
        public sealed record Query(PopulationParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetPlayersPopulationHistoryQuery)}_{Parameters.Key()}");

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
                    PlayerId,
                    SnapshotDate AS Date,
                    Population,
                    ChangePopulation
                FROM PlayersHistory
                WHERE PlayerId IN @Ids
                  AND SnapshotDate >= @FilterDate
                ORDER BY SnapshotDate DESC;";

            var rawHistory = await connection.QueryAsync<dynamic>(new(
                sql,
                new { Ids = ids, FilterDate = filterDate },
                cancellationToken: cancellationToken)
            );

            return rawHistory
                .GroupBy(x => (int)x.PlayerId)
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