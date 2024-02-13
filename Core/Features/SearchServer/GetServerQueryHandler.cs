using Core.Features.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Core.Features.SearchServer
{
    public class GetServerQueryHandler(ServerListDbContext dbContext) : IRequestHandler<GetServerQuery, IList<SearchResult>>
    {
        private readonly ServerListDbContext _dbContext = dbContext;

        public async Task<IList<SearchResult>> Handle(GetServerQuery request, CancellationToken cancellationToken)
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
                .ToListAsync(cancellationToken);
        }
    }
}