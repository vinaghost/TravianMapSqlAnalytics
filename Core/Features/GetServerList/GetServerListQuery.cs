using Core.Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Core.Features.GetServerList
{
    public record GetServerListQuery(ServerListParameters Parameters) : ICachedQuery<IList<ServerRecord>>
    {
        public string CacheKey => $"{nameof(GetServerListQuery)}_{Parameters.Key}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class GetServerListQueryHandler(ServerListDbContext dbContext) : IRequestHandler<GetServerListQuery, IList<ServerRecord>>
    {
        private readonly ServerListDbContext _dbContext = dbContext;

        public async Task<IList<ServerRecord>> Handle(GetServerListQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Servers
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Parameters.SearchTerm))
            {
                query = query.Where(x => x.Url.StartsWith(request.Parameters.SearchTerm));
            }

            return await query
                .OrderBy(x => x.Url)
                .Select(x => new ServerRecord(x.Id, x.Url))
                .ToListAsync(cancellationToken);
        }
    }
}