using System.Data;

using Dapper;

using Features.Shared.Constraints;

using Immediate.Handlers.Shared;

using Microsoft.Extensions.DependencyInjection;

namespace Features.Servers
{
    [Handler]
    public sealed partial class GetMostPlayerServerQuery([FromKeyedServices(DatabaseConnectionType.Servers)] IDbConnection connection)
    {
        public sealed record Query() : DefaultCachedQuery(nameof(GetMostPlayerServerQuery), false);

        private async ValueTask<string> HandleAsync(Query query, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT Url
                FROM Servers
                ORDER BY PlayerCount DESC
                LIMIT 1;";

            var serverUrl = await connection.QueryFirstOrDefaultAsync<string>(
                new CommandDefinition(sql, cancellationToken: cancellationToken)
            );

            return serverUrl ?? "";
        }
    }
}