using Core;
using Features.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Features.SearchAlliance
{
    public class SearchAllianceQueryHandler(ServerDbContext dbContext) : IRequestHandler<SearchAllianceByParametersQuery, IPagedList<SearchResult>>, IRequestHandler<SearchAllianceByIdQuery, IList<SearchResult>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<IPagedList<SearchResult>> Handle(SearchAllianceByParametersQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Alliances
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Parameters.SearchTerm))
            {
                query = query
                    .Where(x => x.Name.StartsWith(request.Parameters.SearchTerm));
            }

            query = query
                .Where(x => x.Players.Count > 0);

            var data = await query
                .OrderBy(x => x.Name)
                .Select(x => new SearchResult(x.Id, string.IsNullOrWhiteSpace(x.Name) ? "No alliance" : x.Name))
                .ToPagedListAsync(request.Parameters.Page, request.Parameters.PageSize);

            return data;
        }

        public async Task<IList<SearchResult>> Handle(SearchAllianceByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Ids.Count == 0) return [];

            var ids = request.Ids.Distinct();
            return await _dbContext.Alliances
                .Where(x => ids.Contains(x.Id))
                .OrderBy(x => x.Name)
                .Select(x => new SearchResult(x.Id, x.Name))
                .ToListAsync(cancellationToken);
        }
    }
}