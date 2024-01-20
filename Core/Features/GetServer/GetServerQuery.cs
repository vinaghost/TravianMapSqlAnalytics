using Core.Features.Shared.Query;
using MediatR;
using X.PagedList;

namespace Core.Features.GetServer
{
    public record GetServerQuery(ServerParameters Parameters) : ICachedQuery<IPagedList<Server>>
    {
        public string CacheKey => $"{nameof(GetServerQuery)}_{Parameters.Key}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class GetServerQueryHandler(ServerListDbContext dbContext) : IRequestHandler<GetServerQuery, IPagedList<Server>>
    {
        private readonly ServerListDbContext _dbContext = dbContext;

        public async Task<IPagedList<Server>> Handle(GetServerQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Servers
                .OrderByDescending(x => x.StartDate)
                .Select(x => new Server(x.Url, x.Zone, x.StartDate, x.AllianceCount, x.PlayerCount, x.VillageCount))
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);
        }
    }
}