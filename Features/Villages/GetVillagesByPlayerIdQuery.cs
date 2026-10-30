using System.Data;

using Dapper;

using Features.Shared.Constraints;

using Immediate.Handlers.Shared;

using Microsoft.Extensions.DependencyInjection;

namespace Features.Villages
{
    [Handler]
    public sealed partial class GetVillagesByPlayerIdQuery([FromKeyedServices(DatabaseConnectionType.Villages)] IDbConnection connection)
    {
        public sealed record Query(int PlayerId) : DefaultCachedQuery($"{nameof(GetVillagesByPlayerIdQuery)}_{PlayerId}");

        private async ValueTask<IList<VillageDto>> HandleAsync(Query query, CancellationToken cancellationToken)
        {
            var playerId = query.PlayerId;

            const string sql = @"
                SELECT
                    MapId,
                    Id AS VillageId,
                    Name AS VillageName,
                    X,
                    Y,
                    Tribe,
                    Population,
                    IsCapital
                FROM Villages
                WHERE PlayerId = @PlayerId AND IsDestroyed = 0;";

            var villages = await connection.QueryAsync<VillageDto>(new(
                sql,
                new { PlayerId = playerId },
                cancellationToken: cancellationToken)
            );

            return [.. villages];
        }
    }
}