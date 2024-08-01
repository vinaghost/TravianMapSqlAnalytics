using Features.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Features.SearchPlayer
{
    public class SearchPlayerQueryHandler(VillageDbContext dbContext) : IRequestHandler<SearchPlayerByParametersQuery, IPagedList<SearchResult>>, IRequestHandler<SearchPlayerByAllianceIdQuery, IList<SearchResult>>
    {
        private readonly VillageDbContext _dbContext = dbContext;

        public async Task<IPagedList<SearchResult>> Handle(SearchPlayerByParametersQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Players
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Parameters.Name))
            {
                query = query
                    .Where(x => x.Name.StartsWith(request.Parameters.Name));
            }

            return await query
                .OrderBy(x => x.Name)
                .Select(x => new SearchResult(x.Id, x.Name))
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);
        }

        public async Task<IList<SearchResult>> Handle(SearchPlayerByAllianceIdQuery request, CancellationToken cancellationToken)
        {
            var id = request.AllianceId;
            if (id == -1)
            {
                return await _dbContext.Players
                    .OrderBy(x => x.Name)
                    .Select(x => new SearchResult(x.Id, x.Name))
                    .ToListAsync(cancellationToken);
            }
            return await _dbContext.Players
                .Where(x => x.AllianceId == id)
                .OrderBy(x => x.Name)
                .Select(x => new SearchResult(x.Id, x.Name))
                .ToListAsync(cancellationToken);
        }
    }
}