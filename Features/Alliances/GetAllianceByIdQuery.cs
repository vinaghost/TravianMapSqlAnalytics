using System.Data;

using Dapper;

using Features.Shared.Constraints;

using Immediate.Handlers.Shared;

using Microsoft.Extensions.DependencyInjection;

namespace Features.Alliances
{
    [Handler]
    public sealed partial class GetAllianceByIdQuery([FromKeyedServices(DatabaseConnectionType.Villages)] IDbConnection connection)
    {
        public sealed record Query(int AllianceId)
            : DefaultCachedQuery($"{nameof(GetAllianceByIdQuery)}_{AllianceId}");

        private async ValueTask<AllianceDto?> HandleAsync(Query query, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT Id, Name, PlayerCount
                FROM Alliances
                WHERE Id = @allianceId
                LIMIT 1;";
            var alliance = await connection.QueryFirstOrDefaultAsync<AllianceDto>(new(
                commandText: sql,
                parameters: new { allianceId = query.AllianceId },
                cancellationToken: cancellationToken
            ));
            return alliance;
        }
    }
}