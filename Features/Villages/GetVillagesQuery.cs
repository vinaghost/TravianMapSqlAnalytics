using System.Data;
using System.Text;

using Dapper;

using Features.Players;
using Features.Shared.Constraints;
using Features.Shared.Parameters;

using FluentValidation;

using Immediate.Handlers.Shared;

using Microsoft.Extensions.DependencyInjection;

using Serilog;

using X.PagedList;

namespace Features.Villages
{
    [Handler]
    public sealed partial class GetVillagesQuery([FromKeyedServices(DatabaseConnectionType.Villages)] IDbConnection connection)
    {
        public sealed record Query(GetVillagesParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetVillagesQuery)}_{Parameters.Key()}");

        private async ValueTask<IPagedList<DetailVillageDto>> HandleAsync(Query query, CancellationToken cancellationToken)
        {
            var parameters = query.Parameters;
            var (whereClause, dynamicParams) = parameters.GetDapperFilter();

            int offset = (parameters.PageNumber - 1) * parameters.PageSize;
            dynamicParams.Add("Offset", offset);
            dynamicParams.Add("PageSize", parameters.PageSize);

            string distanceWhereClause = "1 = 1";
            if (parameters.Distance > 0)
            {
                distanceWhereClause = "Distance <= @DistanceSquared";
            }

            string sql = $@"
                WITH TargetVillages AS (
                    SELECT
                        ((v.X - @X + 601) % 401 - 200) * ((v.X - @X + 601) % 401 - 200) +
                        ((v.Y - @Y + 601) % 401 - 200) * ((v.Y - @Y + 601) % 401 - 200)
                            AS Distance
                    FROM Villages v
                    INNER JOIN Players p ON v.PlayerId = p.Id
                    WHERE {whereClause}
                )
                SELECT COUNT(*)
                FROM TargetVillages
                WHERE {distanceWhereClause};

                WITH TargetVillages AS (
                    SELECT
                        p.AllianceId AS AllianceId,
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
                        v.Population AS Population,
                        ((v.X - @X + 601) % 401 - 200) * ((v.X - @X + 601) % 401 - 200) + ((v.Y - @Y + 601) % 401 - 200) * ((v.Y - @Y + 601) % 401 - 200) AS Distance
                    FROM Villages v
                    INNER JOIN Players p ON v.PlayerId = p.Id
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
                    ROUND(SQRT(Distance), 2) AS Distance
                FROM TargetVillages
                WHERE {distanceWhereClause}
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

    public record GetVillagesParameters : IPaginationParameters, IPlayerFilterParameters, IVillageFilterParameters, IDistanceFilterParameters, IParameters
    {
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 20;

        public int X { get; init; }
        public int Y { get; init; }

        public int Distance { get; init; }

        public int MinPlayerPopulation { get; init; }
        public int MaxPlayerPopulation { get; init; }

        public int MinVillagePopulation { get; init; }
        public int MaxVillagePopulation { get; init; }

        public Capital Capital { get; init; }
        public Tribe Tribe { get; init; }

        public IList<int>? Alliances { get; init; }
        public IList<int>? ExcludeAlliances { get; init; }

        public IList<int>? Players { get; init; }
        public IList<int>? ExcludePlayers { get; init; }

        public string Key()
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '_';

            this.PaginationKey(sb);
            sb.Append(SEPARATOR);
            this.PlayerFilterKey(sb);
            sb.Append(SEPARATOR);
            this.VillageFilterKey(sb);
            sb.Append(SEPARATOR);
            this.DistanceFilterKey(sb);
            return sb.ToString();
        }

        public (string WhereClause, DynamicParameters Parameters) GetDapperFilter()
        {
            var conditions = new List<string>();
            var dynamicParams = new DynamicParameters();

            conditions.Add("v.IsDestroyed = 0");
            conditions.Add("p.IsDeleted = 0");

            if (Players?.Count > 0)
            {
                conditions.Add("p.Id IN @Players");
                dynamicParams.Add("Players", Players);
            }
            else
            {
                if (ExcludePlayers?.Count > 0)
                {
                    conditions.Add("p.Id NOT IN @ExcludePlayers");
                    dynamicParams.Add("ExcludePlayers", ExcludePlayers);
                }

                if (Alliances?.Count > 0)
                {
                    conditions.Add("p.AllianceId IN @Alliances");
                    dynamicParams.Add("Alliances", Alliances);
                }
                else if (ExcludeAlliances?.Count > 0)
                {
                    conditions.Add("p.AllianceId NOT IN @ExcludeAlliances");
                    dynamicParams.Add("ExcludeAlliances", ExcludeAlliances);
                }
            }

            if (MaxPlayerPopulation != 0)
            {
                conditions.Add("p.Population >= @MinPopulation");
                conditions.Add("p.Population <= @MaxPopulation");
                dynamicParams.Add("MinPopulation", MinPlayerPopulation);
                dynamicParams.Add("MaxPopulation", MaxPlayerPopulation);
            }

            if (Capital != Capital.Both)
            {
                if (Capital == Capital.OnlyCapital)
                {
                    conditions.Add("v.IsCapital = @IsCapital");
                    dynamicParams.Add("IsCapital", true);
                }
                else if (Capital == Capital.OnlyVillage)
                {
                    conditions.Add("v.IsCapital = @IsCapital");
                    dynamicParams.Add("IsCapital", false);
                }
            }

            if (Tribe != Tribe.All)
            {
                conditions.Add("v.Tribe = @Tribe");
                dynamicParams.Add("Tribe", (int)Tribe);
            }

            if (MaxVillagePopulation != 0)
            {
                conditions.Add("v.Population >= @MinVillagePopulation");
                conditions.Add("v.Population <= @MaxVillagePopulation");
                dynamicParams.Add("MinVillagePopulation", MinVillagePopulation);
                dynamicParams.Add("MaxVillagePopulation", MaxVillagePopulation);
            }

            //conditions.Add("((v.X - @X + 601) % 401 - 200) * ((v.X - @X + 601) % 401 - 200) + ((v.Y - @Y + 601) % 401 - 200) * ((v.Y - @Y + 601) % 401 - 200) <= @DistanceSquared");
            dynamicParams.Add("X", X);
            dynamicParams.Add("Y", Y);
            dynamicParams.Add("DistanceSquared", Distance * Distance);

            string whereClause = conditions.Count > 0 ? string.Join(" AND ", conditions) : "1=1";

            return (whereClause, dynamicParams);
        }
    }

    public class GetVillagesParametersValidator : AbstractValidator<GetVillagesParameters>
    {
        public GetVillagesParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new DistanceFilterParametersValidator());
            Include(new PlayerFilterParametersValidator());
            Include(new VillageFilterParametersValidator());
        }
    }
}