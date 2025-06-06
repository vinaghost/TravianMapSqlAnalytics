using Features.Shared.Constraints;
using FluentValidation;
using Immediate.Handlers.Shared;
using Infrastructure.DbContexts;
using Infrastructure.Entities;
using LinqKit;
using X.PagedList;
using X.PagedList.Extensions;

namespace Features.Alliances.GetAlliancesByName
{
    [Handler]
    public static partial class GetAlliancesByNameQuery
    {
        public sealed record Query(GetAlliancesByNameParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetAlliancesByNameQuery)}_{Parameters.Key()}");

        private static ValueTask<IPagedList<AllianceDto>> HandleAsync(
            Query query,
            VillageDbContext context,
            CancellationToken cancellationToken
        )
        {
            var parameters = query.Parameters;

            var predicate = PredicateBuilder.New<Alliance>();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                predicate = predicate.And(x => x.Name.StartsWith(parameters.SearchTerm));
            }

            predicate = predicate.And(x => x.Players.Count > 0);

            var data = context.Alliances
                .Where(predicate)
                .OrderBy(x => x.Name)
                .Select(x => new AllianceDto(
                    x.Id,
                    string.IsNullOrWhiteSpace(x.Name) ? "No alliance" : x.Name,
                    x.PlayerCount
                ))
                .ToPagedList(parameters.PageNumber, parameters.PageSize);

            return ValueTask.FromResult(data);
        }
    }
}