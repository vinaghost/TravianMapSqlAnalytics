using Features.Shared.Parameters;
using Features.Shared.Query;
using FluentValidation;
using MediatR;
using System.Text;
using X.PagedList;

namespace Features.Servers
{
    public record GetServersByNameParameters : ISearchTermParameters, IPaginationParameters
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public string? Name { get; init; }
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
            sb.Append(parameters.Name);

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

            if (!string.IsNullOrWhiteSpace(parameters.Name))
            {
                query = query
                    .Where(x => x.Url.StartsWith(parameters.Name));
            }

            var data = await query
               .OrderBy(x => x.Url)
               .Select(x => new ServerDto(x.Id, x.Url))
               .ToPagedListAsync(parameters.PageNumber, parameters.PageSize);
            return data;
        }
    }
}