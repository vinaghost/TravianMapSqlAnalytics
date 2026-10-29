using System.Data;

using Dapper;

using Features.Shared.Constraints;

using Immediate.Handlers.Shared;

using Infrastructure;

using Microsoft.Extensions.DependencyInjection;

namespace Features.Servers
{
    [Handler]
    public sealed partial class IsValidServerUrlQuery([FromKeyedServices(DatabaseConnectionType.Servers)] IDbConnection connection)
    {
        public sealed record Query(string ServerUrl) : DefaultCachedQuery($"{nameof(IsValidServerUrlQuery)}_{ServerUrl}", false);

        private async ValueTask<bool> HandleAsync(Query query, CancellationToken cancellationToken)
        {
            var serverUrl = query.ServerUrl;

            const string sql = @"
                SELECT 1
                FROM Servers
                WHERE Url = @ServerUrl
                LIMIT 1;";

            var exists = await connection.ExecuteScalarAsync<int?>(new(
                sql,
                new { ServerUrl = serverUrl },
                cancellationToken: cancellationToken)
            );

            return exists.HasValue;
        }
    }
}