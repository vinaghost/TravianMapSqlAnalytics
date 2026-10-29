using System.Data;
using System.Text;

using Dapper;

using Features.Shared.Constraints;
using Features.Shared.Parameters;

using FluentValidation;

using Immediate.Handlers.Shared;

using Infrastructure;

using Microsoft.Extensions.DependencyInjection;

using X.PagedList;

namespace Features.Servers
{
    public record GetServersByNameParameters : ISearchTermParameters, IPaginationParameters
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public string? SearchTerm { get; init; }
    }

    public static class GetServersByNameParametersExtension
    {
        public static string Key(this GetServersByNameParameters parameters)
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '_';

            sb.Append(parameters.PageNumber);
            sb.Append(SEPARATOR);
            sb.Append(parameters.PageSize);
            sb.Append(SEPARATOR);
            sb.Append(parameters.SearchTerm);

            return sb.ToString();
        }
    }

    public class GetServersByNameParametersValidator : AbstractValidator<GetServersByNameParameters>;

    [Handler]
    public sealed partial class GetServersByNameQuery([FromKeyedServices(DatabaseConnectionType.Servers)] IDbConnection connection)
    {
        public sealed record Query(GetServersByNameParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetServersByNameQuery)}_{Parameters.Key()}", false);

        private async ValueTask<IPagedList<ServerDto>> HandleAsync(Query query, CancellationToken cancellationToken)
        {
            var parameters = query.Parameters;

            var conditions = new List<string>();
            var dynamicParams = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                conditions.Add("Url LIKE @SearchTerm");
                dynamicParams.Add("SearchTerm", $"{parameters.SearchTerm}%");
            }

            string whereClause = conditions.Count > 0 ? string.Join(" AND ", conditions) : "1=1";

            int offset = (parameters.PageNumber - 1) * parameters.PageSize;
            dynamicParams.Add("Offset", offset);
            dynamicParams.Add("PageSize", parameters.PageSize);

            string sql = $@"
                SELECT COUNT(*) FROM Servers WHERE {whereClause};

                SELECT Id, Url
                FROM Servers
                WHERE {whereClause}
                ORDER BY Url
                LIMIT @PageSize OFFSET @Offset;";

            using var multi = await connection.QueryMultipleAsync(
                new CommandDefinition(sql, dynamicParams, cancellationToken: cancellationToken)
            );

            int totalCount = await multi.ReadFirstAsync<int>();
            var servers = await multi.ReadAsync<ServerDto>();

            var data = new StaticPagedList<ServerDto>(servers, parameters.PageNumber, parameters.PageSize, totalCount);
            return data;
        }
    }
}