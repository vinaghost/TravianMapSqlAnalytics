using Core.Features.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Features.SearchAlliance
{
    public class GetAllianceQueryHandler(ServerDbContext dbContext) : IRequestHandler<GetAllianceQuery, IList<SearchResult>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<IList<SearchResult>> Handle(GetAllianceQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Alliances
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Parameters.SearchTerm))
            {
                query = query.Where(x => x.Name.StartsWith(request.Parameters.SearchTerm));
            }

            return await query
                .OrderBy(x => x.Name)
                .Select(x => new SearchResult(x.Id, x.Name))
                .ToListAsync(cancellationToken);
        }
    }
}