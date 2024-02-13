using Core.Features.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Features.SearchPlayer
{
    public class GetPlayerQueryHandler(ServerDbContext dbContext) : IRequestHandler<GetPlayerQuery, IList<SearchResult>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<IList<SearchResult>> Handle(GetPlayerQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Players
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