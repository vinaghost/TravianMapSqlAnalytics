using Features.Shared.Parameters;
using Features.Shared.Query;
using MediatR;
using X.PagedList;

namespace Features.Alliances
{
    public record GetAllAllianceQuery(NameFilterParameters Parameters) : ICachedQuery<IPagedList<AllianceDto>>
    {
        public string CacheKey => $"{nameof(GetAllAllianceQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetAllAllianceQueryHandler(VillageDbContext dbContext) : IRequestHandler<GetAllAllianceQuery, IPagedList<AllianceDto>>
    {
        private readonly VillageDbContext _dbContext = dbContext;

        public async Task<IPagedList<AllianceDto>> Handle(GetAllAllianceQuery request, CancellationToken cancellationToken)
        {
            var query = _dbContext.Alliances
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Parameters.Name))
            {
                query = query
                    .Where(x => x.Name.StartsWith(request.Parameters.Name));
            }

            query = query
                .Where(x => x.Players.Count > 0);

            var data = await query
                .OrderBy(x => x.Name)
                .Select(x => new AllianceDto(
                        x.Id,
                        string.IsNullOrWhiteSpace(x.Name) ? "No alliance" : x.Name,
                        x.PlayerCount
                ))
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);

            return data;
        }
    }
}