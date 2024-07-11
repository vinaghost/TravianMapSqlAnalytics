
using Features.Shared.Query;
using MediatR;
using X.PagedList;

namespace Features.GetServer
{
    public record GetServerQuery(ServerParameters Parameters) : ICachedQuery<IPagedList<Server>>
    {
        public string CacheKey => $"{nameof(GetServerQuery)}_{Parameters.Key}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class GetServerQueryHandler(ServerDbContext dbContext) : IRequestHandler<GetServerQuery, IPagedList<Server>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<IPagedList<Server>> Handle(GetServerQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Servers
                .OrderByDescending(x => x.Id)
                .Select(x => new Server(x.Url, x.AllianceCount, x.PlayerCount, x.VillageCount))
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);
        }
    }
}