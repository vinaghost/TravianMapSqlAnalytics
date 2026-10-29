using System.Data;

using Dapper;

using Features.Shared.Constraints;
using Features.Shared.Parameters;

using FluentValidation;

using Immediate.Handlers.Shared;

using Microsoft.Extensions.DependencyInjection;

namespace Features.Alliances
{
    public record GetAlliancesByIdParameters(IList<int> Ids) : IParameters
    {
        public string Key() => string.Join(',', Ids.Distinct().Order());
    }

    [Handler]
    public sealed partial class GetAlliancesByIdQuery([FromKeyedServices(DatabaseConnectionType.Villages)] IDbConnection connection)
    {
        public sealed record Query(GetAlliancesByIdParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetAlliancesByIdQuery)}_{Parameters.Key()}");

        private async ValueTask<IList<AllianceDto>> HandleAsync(Query query, CancellationToken cancellationToken)
        {
            var ids = query.Parameters.Ids.Distinct().ToList();
            if (ids.Count == 0) return [];

            const string sql = @"
                SELECT Id, Name, PlayerCount
                FROM Alliances
                WHERE Id IN @Ids";

            var alliances = await connection.QueryAsync<AllianceDto>(new(
                sql,
                new { Ids = ids },
                cancellationToken: cancellationToken)
            );

            return [.. alliances];
        }
    }

    public class GetAlliancesByIdParametersValidator : AbstractValidator<GetAlliancesByIdParameters>
    {
        public GetAlliancesByIdParametersValidator()
        {
            RuleFor(x => x.Ids).NotEmpty();
        }
    }
}