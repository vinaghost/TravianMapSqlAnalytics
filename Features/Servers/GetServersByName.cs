using Features.Shared.Dtos;
using Features.Shared.Parameters;
using Features.Shared.Query;
using System.Text;

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

    public record GetServersByNameQuery(GetServersByNameParameters Parameters) : ICachedQuery<IPagedList<ServerDto>>
    {
        public string CacheKey => $"{nameof(GetServersByNameQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class GetServersByNameQueryHandler(ServerDbContext dbContext) : IRequestHandler<GetServersByNameQuery, IPagedList<ServerDto>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<IPagedList<ServerDto>> Handle(GetServersByNameQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;
            var query = _dbContext.Servers
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                query = query
                    .Where(x => x.Url.StartsWith(parameters.SearchTerm));
            }

            var data = query
               .OrderBy(x => x.Url)
               .Select(x => new ServerDto(x.Id, x.Url))
               .ToPagedList(parameters.PageNumber, parameters.PageSize);
            return data;
        }
    }
}