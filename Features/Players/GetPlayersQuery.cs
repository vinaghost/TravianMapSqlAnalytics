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

namespace Features.Players
{
    public record GetPlayersParameters : IPaginationParameters, IPlayerFilterParameters, IParameters
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public int MinPlayerPopulation { get; init; }
        public int MaxPlayerPopulation { get; init; }

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

            return sb.ToString();
        }
    }

    [Handler]
    public sealed partial class GetPlayersQuery([FromKeyedServices(DatabaseConnectionType.Villages)] IDbConnection connection)
    {
        public sealed record Query(GetPlayersParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetPlayersQuery)}_{Parameters.Key()}");

        private async ValueTask<IPagedList<PlayerDto>> HandleAsync(Query query, CancellationToken cancellationToken)
        {
            var parameters = query.Parameters;
            var (whereClause, dynamicParams) = parameters.GetDapperFilter();

            int offset = (parameters.PageNumber - 1) * parameters.PageSize;
            dynamicParams.Add("Offset", offset);
            dynamicParams.Add("PageSize", parameters.PageSize);

            string sql = $@"
                SELECT COUNT(*)
                FROM Players p
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
                ORDER BY p.VillageCount DESC
                LIMIT @PageSize OFFSET @Offset;";

            using var multi = await connection.QueryMultipleAsync(new(
                sql,
                dynamicParams,
                cancellationToken: cancellationToken)
            );

            int totalCount = await multi.ReadFirstAsync<int>();
            var playersList = await multi.ReadAsync<PlayerDto>();

            var data = new StaticPagedList<PlayerDto>(playersList, parameters.PageNumber, parameters.PageSize, totalCount);

            return data;
        }
    }

    public class GetPlayersParametersValidator : AbstractValidator<GetPlayersParameters>
    {
        public GetPlayersParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new PlayerFilterParametersValidator());
        }
    }
}