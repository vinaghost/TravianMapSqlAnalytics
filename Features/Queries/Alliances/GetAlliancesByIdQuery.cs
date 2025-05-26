using Features.Constraints;
using Features.Dtos;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Features.Queries.Alliances
{
    public record GetAlliancesByIdParameters(IList<int> Ids);

    public static class GetAlliancesByIdParametersExtension
    {
        public static string Key(this GetAlliancesByIdParameters parameters)
        {
            return string.Join(',', parameters.Ids.Distinct().Order());
        }
    }

    public class GetAlliancesByIdParametersValidator : AbstractValidator<GetAlliancesByIdParameters>
    {
        public GetAlliancesByIdParametersValidator()
        {
            RuleFor(x => x.Ids).NotEmpty();
        }
    }

    [Handler]
    public static partial class GetAlliancesByIdQuery
    {
        public sealed record Query(GetAlliancesByIdParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetAlliancesByIdQuery)}_{Parameters.Key()}");

        private static async ValueTask<IList<AllianceDto>> HandleAsync(
            Query query,
            VillageDbContext context,
            CancellationToken cancellationToken
        )
        {
            var ids = query.Parameters.Ids.Distinct().ToList();
            if (ids.Count == 0) return [];

            var queryable = context.Alliances.AsQueryable();

            if (ids.Count == 1)
            {
                queryable = queryable.Where(x => x.Id == ids[0]);
            }
            else
            {
                queryable = queryable.Where(x => ids.Contains(x.Id));
            }

            var alliances = await queryable
                .Select(x => new AllianceDto(
                    x.Id,
                    x.Name,
                    x.PlayerCount
                ))
                .ToListAsync(cancellationToken);

            return alliances;
        }
    }
}