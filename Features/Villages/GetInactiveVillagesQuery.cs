using System.Data;

using Dapper;

using Features.Shared.Constraints;

using FluentValidation;

using Immediate.Handlers.Shared;

using Microsoft.Extensions.DependencyInjection;

using X.PagedList;

namespace Features.Villages
{
    [Handler]
    public sealed partial class GetInactiveVillagesQuery([FromKeyedServices(DatabaseConnectionType.Villages)] IDbConnection connection)
    {
        public sealed record Query(GetInactiveVillagesParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetInactiveVillagesQuery)}_{Parameters.Key()}");

        private async ValueTask<IPagedList<DetailVillageDto>> HandleAsync(Query query, CancellationToken cancellationToken)
        {
            var parameters = query.Parameters;
            var (whereClause, dynamicParams) = parameters.GetDapperFilter();

            var inactiveDate = DateTime.Today.AddDays(-parameters.InactiveDays);
            dynamicParams.Add("InactiveDays", parameters.InactiveDays);
            dynamicParams.Add("Date", inactiveDate);

            int offset = (parameters.PageNumber - 1) * parameters.PageSize;
            dynamicParams.Add("Offset", offset);
            dynamicParams.Add("PageSize", parameters.PageSize);

            string sql = $@"
                WITH InactivePlayers AS (
                    SELECT PlayerId
                    FROM PlayersHistory
                    WHERE Date >= @Date
                    GROUP BY PlayerId
                    HAVING COUNT(*) >= @InactiveDays
                        AND MAX(ChangePopulation) = 0
                        AND MIN(ChangePopulation) = 0
                ),
                TargetVillages AS (
                    SELECT
                        ((vil.X - @X + 601) % 401 - 200) * ((vil.X - @X + 601) % 401 - 200) +
                        ((vil.Y - @Y + 601) % 401 - 200) * ((vil.Y - @Y + 601) % 401 - 200)
                            AS Distance
                    FROM Villages v
                    INNER JOIN Players p ON v.PlayerId = p.Id
                    INNER JOIN InactivePlayers ip ON p.Id = ip.PlayerId
                    WHERE {whereClause}
                )
                SELECT COUNT(*)
                FROM TargetVillages
                WHERE (@DistanceSquared = 0 OR Distance <= @DistanceSquared);

                WITH InactivePlayers AS (
                    SELECT PlayerId
                    FROM PlayersHistory
                    WHERE Date >= @Date
                    GROUP BY PlayerId
                    HAVING COUNT(*) >= @InactiveDays
                       AND MAX(ChangePopulation) = 0
                       AND MIN(ChangePopulation) = 0
                ),
                TargetVillages AS (
                    SELECT
                        v.AllianceId AS AllianceId,
                        a.Name AS AllianceName,
                        v.PlayerId AS PlayerId,
                        p.Name AS PlayerName,
                        v.Id AS VillageId,
                        v.MapId AS MapId,
                        v.Name AS VillageName,
                        v.X AS X,
                        v.Y AS Y,
                        v.IsCapital AS IsCapital,
                        v.Tribe AS Tribe,
                        v.Population AS Population
                        ((v.X - @X + 601) % 401 - 200) * ((v.X - @X + 601) % 401 - 200) + ((v.Y - @Y + 601) % 401 - 200) * ((v.Y - @Y + 601) % 401 - 200) AS Distance
                    FROM Villages v
                    INNER JOIN Players p ON v.PlayerId = p.Id
                    INNER JOIN InactivePlayers ip ON p.Id = ip.PlayerId
                    LEFT JOIN Alliances a ON p.AllianceId = a.Id
                    WHERE {whereClause}
                )
                SELECT
                    AllianceId,
                    AllianceName,
                    PlayerId,
                    PlayerName,
                    VillageId,
                    MapId,
                    VillageName,
                    X,
                    Y,
                    IsCapital,
                    Tribe,
                    Population,
                    Distance
                FROM TargetVillages
                WHERE (@DistanceSquared = 0 OR Distance <= @DistanceSquared)
                ORDER BY Distance
                LIMIT @PageSize OFFSET @Offset;";

            using var multi = await connection.QueryMultipleAsync(new(
                sql,
                dynamicParams,
                cancellationToken: cancellationToken)
            );

            int totalCount = await multi.ReadFirstAsync<int>();
            var playersList = await multi.ReadAsync<DetailVillageDto>();

            var data = new StaticPagedList<DetailVillageDto>(playersList, parameters.PageNumber, parameters.PageSize, totalCount);
            return data;
        }
    }

    public record GetInactiveVillagesParameters : GetVillagesParameters
    {
        public int InactiveDays { get; init; } = 3;
    }

    public class GetInactiveVillagesParametersValidator : AbstractValidator<GetInactiveVillagesParameters>
    {
        public GetInactiveVillagesParametersValidator()
        {
            Include(new GetVillagesParametersValidator());

            RuleFor(x => x.InactiveDays)
                .NotEmpty()
                .GreaterThanOrEqualTo(3);
        }
    }
}