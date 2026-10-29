using System.Data;
using System.Text;

using Dapper;

using Features.Players;
using Features.Shared.Constraints;
using Features.Shared.Parameters;

using FluentValidation;

using Immediate.Handlers.Shared;

using Infrastructure;

using Microsoft.Extensions.DependencyInjection;

using X.PagedList;

namespace FeaturesFeatures.Players
{
    public record GetPlayersByNameParameters : IPaginationParameters, ISearchTermParameters, IParameters
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public string? SearchTerm { get; init; }

        public string Key()
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '_';

            this.PaginationKey(sb);
            sb.Append(SEPARATOR);
            this.SearchTermKey(sb);

            return sb.ToString();
        }
    }

    [Handler]
    public sealed partial class GetPlayersByNameQuery([FromKeyedServices(DatabaseConnectionType.Villages)] IDbConnection connection)
    {
        public sealed record Query(GetPlayersByNameParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetPlayersByNameQuery)}_{Parameters.Key()}");

        private async ValueTask<IPagedList<PlayerDto>> HandleAsync(Query query, CancellationToken cancellationToken)
        {
            var parameters = query.Parameters;
            // 1. Setup dynamic where clause conditions
            var conditions = new List<string>();
            var dynamicParams = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                conditions.Add("p.Name LIKE @SearchTerm");
                dynamicParams.Add("SearchTerm", $"{parameters.SearchTerm}%");
            }

            string whereClause = conditions.Count > 0 ? string.Join(" AND ", conditions) : "1=1";

            int offset = (parameters.PageNumber - 1) * parameters.PageSize;
            dynamicParams.Add("Offset", offset);
            dynamicParams.Add("PageSize", parameters.PageSize);

            string sql = $@"
                SELECT COUNT(*)
                FROM Players p
                INNER JOIN Alliances a ON p.AllianceId = a.Id
                WHERE {whereClause};

                SELECT
                    a.Id AS AllianceId,
                    a.Name AS AllianceName,
                    p.Id AS PlayerId,
                    p.Name AS PlayerName,
                    p.VillageCount,
                    p.Population
                FROM Players p
                INNER JOIN Alliances a ON p.AllianceId = a.Id
                WHERE {whereClause}
                ORDER BY p.Name
                LIMIT @PageSize OFFSET @Offset;";

            using var multi = await connection.QueryMultipleAsync(new(
                sql,
                dynamicParams,
                cancellationToken: cancellationToken)
            );

            int totalCount = await multi.ReadFirstAsync<int>();
            var players = await multi.ReadAsync<PlayerDto>();

            return new StaticPagedList<PlayerDto>(players, parameters.PageNumber, parameters.PageSize, totalCount);
        }
    }

    public class GetPlayersByNameParametersValidator : AbstractValidator<GetPlayersByNameParameters>
    {
        public GetPlayersByNameParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new SearchTermParametersValidator());
        }
    }
}