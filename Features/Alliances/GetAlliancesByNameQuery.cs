using System.Data;
using System.Text;

using Dapper;

using Features.Shared.Constraints;
using Features.Shared.Parameters;

using FluentValidation;

using Immediate.Handlers.Shared;

using Microsoft.Extensions.DependencyInjection;

using X.PagedList;

namespace Features.Alliances
{
    public record GetAlliancesByNameParameters : IPaginationParameters, ISearchTermParameters, IParameters
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public string? SearchTerm { get; init; }

        public string Key()
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '|';

            this.PaginationKey(sb);
            sb.Append(SEPARATOR);
            this.SearchTermKey(sb);

            return sb.ToString();
        }
    }

    [Handler]
    public sealed partial class GetAlliancesByNameQuery([FromKeyedServices(DatabaseConnectionType.Villages)] IDbConnection connection)
    {
        public sealed record Query(GetAlliancesByNameParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetAlliancesByNameQuery)}_{Parameters.Key()}");

        private async ValueTask<IPagedList<AllianceDto>> HandleAsync(Query query, CancellationToken cancellationToken)
        {
            var parameters = query.Parameters;

            var conditions = new List<string> { "PlayerCount > 0" };
            var dynamicParams = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                conditions.Add("Name LIKE @SearchTerm");
                dynamicParams.Add("SearchTerm", $"{parameters.SearchTerm}%");
            }

            string whereClause = string.Join(" AND ", conditions);

            int offset = (parameters.PageNumber - 1) * parameters.PageSize;
            dynamicParams.Add("Offset", offset);
            dynamicParams.Add("PageSize", parameters.PageSize);

            string sql = $@"
                SELECT COUNT(*) FROM Alliances WHERE {whereClause};

                SELECT
                    Id AS AllianceId,
                    Name AS AllianceName,
                    PlayerCount
                FROM Alliances
                WHERE {whereClause}
                ORDER BY Name
                LIMIT @PageSize OFFSET @Offset;";

            using var multi = await connection.QueryMultipleAsync(new(
                sql,
                dynamicParams,
                cancellationToken: cancellationToken)
            );

            int totalCount = await multi.ReadFirstAsync<int>();
            var alliances = await multi.ReadAsync<AllianceDto>();

            return new StaticPagedList<AllianceDto>(alliances, parameters.PageNumber, parameters.PageSize, totalCount);
        }
    }

    public class GetAllianceByNameParametersValidator : AbstractValidator<GetAlliancesByNameParameters>
    {
        public GetAllianceByNameParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new SearchTermParametersValidator());
        }
    }
}