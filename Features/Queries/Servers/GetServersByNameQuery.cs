using Features.Constraints;
using Features.Dtos;
using Features.Parameters;
using Immediate.Handlers.Shared;
using System.Text;

namespace Features.Queries.Servers
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
    public static partial class GetServersByNameQuery
    {
        public sealed record Query(GetServersByNameParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetServersByNameQuery)}_{Parameters.Key()}", false);

        private static ValueTask<IPagedList<ServerDto>> HandleAsync(
            Query query,
            ServerDbContext context,
            CancellationToken cancellationToken
        )
        {
            var parameters = query.Parameters;

            var predicate = PredicateBuilder.New<Server>(true);
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                predicate = predicate.And(x => x.Url.StartsWith(parameters.SearchTerm));
            }

            var data = context.Servers
                .AsQueryable()
                .Where(predicate)
                .OrderBy(x => x.Url)
                .Select(x => new ServerDto(x.Id, x.Url))
                .ToPagedList(parameters.PageNumber, parameters.PageSize);

            return ValueTask.FromResult(data);
        }
    }
}