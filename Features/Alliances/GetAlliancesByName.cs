using Features.Shared.Dtos;
using Features.Shared.Parameters;
using Features.Shared.Query;
using System.Text;

namespace Features.Alliances
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

    public record GetAlliancesByNameQuery(GetAlliancesByNameParameters Parameters) : ICachedQuery<IPagedList<AllianceDto>>
    {
        public string CacheKey => $"{nameof(GetAlliancesByNameQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetAlliancesByNameQueryHandler(VillageDbContext dbContext) : IRequestHandler<GetAlliancesByNameQuery, IPagedList<AllianceDto>>
    {
        private readonly VillageDbContext _dbContext = dbContext;

        public async Task<IPagedList<AllianceDto>> Handle(GetAlliancesByNameQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;

            var predicate = PredicateBuilder.New<Alliance>(true);

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                predicate = predicate.And(x => x.Name.StartsWith(parameters.SearchTerm));
            }

            predicate = predicate.And(x => x.Players.Count > 0);

            var data = _dbContext.Alliances
                .Where(predicate)
                .OrderBy(x => x.Name)
                .Select(x => new AllianceDto(
                        x.Id,
                        string.IsNullOrWhiteSpace(x.Name) ? "No alliance" : x.Name,
                        x.PlayerCount
                ))
                .ToPagedList(parameters.PageNumber, parameters.PageSize);

            return data;
        }
    }
}