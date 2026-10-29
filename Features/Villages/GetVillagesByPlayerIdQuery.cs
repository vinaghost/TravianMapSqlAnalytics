using System.Data;

using Dapper;

using Features.Shared.Constraints;

using Immediate.Handlers.Shared;

using Infrastructure;

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
                    Id,
                    Name,
                    X,
                    Y,
                    Tribe,
                    Population,
                    IsCapital
                FROM Villages
                WHERE PlayerId = @PlayerId;";

            var villages = await connection.QueryAsync<VillageDto>(new(
                sql,
                new { PlayerId = playerId },
                cancellationToken: cancellationToken)
            );

            return [.. villages];

            //            var rawRows = await connection.QueryAsync<dynamic>(
            //    new CommandDefinition(sql, new { PlayerId = playerId }, cancellationToken: cancellationToken)
            //);

            //            return rawRows.Select(x => new VillageDto(
            //                (int)x.MapId,
            //                (int)x.Id,
            //                (string)x.Name,
            //                (int)x.X,
            //                (int)x.Y,
            //                (Tribe)x.Tribe, // Explicitly cast the database integer to your Tribe enum
            //                (int)x.Population,
            //                (bool)x.IsCapital
            //            )).ToList();
        }
    }
}