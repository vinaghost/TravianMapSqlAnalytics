using Features.Dtos;
using Features.Parameters;
using Features.Shared.Constraints;
using FluentValidation;
using Immediate.Handlers.Shared;
using Infrastructure.DbContexts;
using Infrastructure.Entities;
using LinqKit;
using System.Text;
using X.PagedList;
using X.PagedList.Extensions;

namespace Features.Queries.Alliances
{
    public record GetAlliancesByNameParameters : IPaginationParameters, ISearchTermParameters
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public string? SearchTerm { get; init; }
    }

    public static class GetAlliancesByNameParametersExtension
    {
        public static string Key(this GetAlliancesByNameParameters parameters)
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '_';

            parameters.PaginationKey(sb);
            sb.Append(SEPARATOR);
            parameters.SearchTermKey(sb);

            return sb.ToString();
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