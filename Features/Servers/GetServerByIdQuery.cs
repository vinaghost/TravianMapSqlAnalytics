using System.Data;

using Dapper;

using Features.Shared.Constraints;

using Immediate.Handlers.Shared;

using Infrastructure;

using Microsoft.Extensions.DependencyInjection;

namespace Features.Servers
{
    [Handler]
    public sealed partial class GetServerByIdQuery([FromKeyedServices(DatabaseConnectionType.Servers)] IDbConnection connection)
    {
        public sealed record Query(int ServerId) :
            DefaultCachedQuery($"{nameof(GetServerByIdQuery)}_{ServerId}", false);

        private async ValueTask<ServerDto?> HandleAsync(Query query, CancellationToken cancellationToken)
        {
            var serverId = query.ServerId;

            const string sql = @"
                SELECT Id, Url
                FROM Servers
                WHERE Id = @ServerId
                LIMIT 1;";

            var server = await connection.QueryFirstOrDefaultAsync<ServerDto>(new(
                sql,
                new { ServerId = serverId },
                cancellationToken: cancellationToken)
            );

            return server;
        }
    }
}