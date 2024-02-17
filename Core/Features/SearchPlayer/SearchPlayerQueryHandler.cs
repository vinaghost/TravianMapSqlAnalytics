﻿using Core.Features.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Core.Features.SearchPlayer
{
    public class SearchPlayerQueryHandler(ServerDbContext dbContext) : IRequestHandler<SearchPlayerByParametersQuery, IPagedList<SearchResult>>, IRequestHandler<SearchPlayerByIdQuery, IList<SearchResult>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<IPagedList<SearchResult>> Handle(SearchPlayerByParametersQuery request, CancellationToken cancellationToken)
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
                .ToPagedListAsync(request.Parameters.Page, request.Parameters.PageSize);
        }

        public async Task<IList<SearchResult>> Handle(SearchPlayerByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Ids.Count == 0) return [];
            var ids = request.Ids.Distinct();
            return await _dbContext.Players
                .Where(x => ids.Contains(x.Id))
                .OrderBy(x => x.Name)
                .Select(x => new SearchResult(x.Id, x.Name))
                .ToListAsync(cancellationToken);
        }
    }
}