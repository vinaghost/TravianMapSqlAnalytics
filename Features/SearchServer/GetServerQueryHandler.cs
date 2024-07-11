
using Features.Shared.Models;
using MediatR;
using X.PagedList;

namespace Features.SearchServer
{
    public class GetServerQueryHandler(ServerDbContext dbContext) : IRequestHandler<GetServerQuery, IPagedList<SearchResult>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<IPagedList<SearchResult>> Handle(GetServerQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Servers
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Parameters.SearchTerm))
            {
                query = query.Where(x => x.Url.StartsWith(request.Parameters.SearchTerm));
            }

            return await query
                .OrderBy(x => x.Url)
                .Select(x => new SearchResult(x.Id, x.Url))
                .ToPagedListAsync(request.Parameters.Page, request.Parameters.PageSize);
        }
    }
}