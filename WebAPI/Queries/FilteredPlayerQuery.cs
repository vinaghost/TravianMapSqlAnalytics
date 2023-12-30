using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PlayerEnitty = Core.Models.Player;

namespace WebAPI.Queries
{
    public interface IPlayerFilterdRequest
    {
        List<int> Alliances { get; }
    }

    public record FilteredPlayerQuery(List<int> Alliances) : IQuery<IQueryable<PlayerEnitty>>, IPlayerFilterdRequest;
    public record GetPlayerCountQuery(List<int> Alliances) : ICachedQuery<int>, IPlayerFilterdRequest
    {
        public string CacheKey => $"player_count_{string.Join(',', Alliances)}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class PlayerQueryHandler(ServerDbContext dbContext) : IRequestHandler<FilteredPlayerQuery, IQueryable<PlayerEnitty>>, IRequestHandler<GetPlayerCountQuery, int>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<IQueryable<PlayerEnitty>> Handle(FilteredPlayerQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var filterQuery = GetQuery(request);
            return filterQuery;
        }

        public async Task<int> Handle(GetPlayerCountQuery request, CancellationToken cancellationToken)
        {
            var filterQuery = GetQuery(request);
            return await filterQuery.CountAsync(cancellationToken: cancellationToken);
        }

        private IQueryable<PlayerEnitty> GetQuery(IPlayerFilterdRequest request)
        {
            if (request.Alliances.Count > 0)
            {
                return _dbContext.Alliances
                     .Where(x => request.Alliances.Contains(x.AllianceId))
                     .SelectMany(x => x.Players);
            }
            else
            {
                return _dbContext.Players.AsQueryable();
            }
        }
    }
}