using System.Data;

using Dapper;

using Features.Shared.Constraints;

using Immediate.Handlers.Shared;

using Microsoft.Extensions.DependencyInjection;

namespace Features.Servers
{
    [Handler]
    public sealed partial class GetServerCountQuery([FromKeyedServices(DatabaseConnectionType.Servers)] IDbConnection connection)
    {
        public sealed record Query() : DefaultCachedQuery(nameof(GetServerCountQuery), false);

        private async ValueTask<int> HandleAsync(Query query, CancellationToken cancellationToken)
        {
            const string sql = "SELECT COUNT(*) FROM Servers;";

            var count = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, cancellationToken: cancellationToken)
            );

            return count;
        }
    }
}