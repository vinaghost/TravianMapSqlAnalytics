using System.Data;

using Dapper;

using Features.Shared.Constraints;

using Immediate.Handlers.Shared;

using Microsoft.Extensions.DependencyInjection;

namespace Features.Players
{
    [Handler]
    public sealed partial class GetPlayerByIdQuery([FromKeyedServices(DatabaseConnectionType.Villages)] IDbConnection connection)
    {
        public sealed record Query(int PlayerId)
            : DefaultCachedQuery($"{nameof(GetPlayerByIdQuery)}_{PlayerId}");

        private async ValueTask<PlayerDto?> HandleAsync(Query query, CancellationToken cancellationToken)
        {
            var playerId = query.PlayerId;
            const string sql = @"
                SELECT
                    a.Id AS AllianceId,
                    a.Name AS AllianceName,
                    p.Id AS PlayerId,
                    p.Name AS PlayerName,
                    p.VillageCount,
                    p.Population
                FROM Players p
                INNER JOIN Alliances a ON p.AllianceId = a.Id
                WHERE p.Id = @PlayerId
                LIMIT 1;";

            var player = await connection.QueryFirstOrDefaultAsync<PlayerDto>(new(
                sql,
                new { PlayerId = playerId },
                cancellationToken: cancellationToken)
            );
            return player;
        }
    }
}