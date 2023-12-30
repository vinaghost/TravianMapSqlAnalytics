using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Queries
{
    public record PlayerInfoQuery(List<int> Ids) : ICachedQuery<Dictionary<int, PlayerInfo>>
    {
        private readonly List<int> ids = Ids;
        public List<int> Ids => ids.Distinct().Order().ToList();
        public string CacheKey => $"{nameof(PlayerInfoQuery)}_{string.Join(',', Ids)}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class PlayerNameQueryHandler(ServerDbContext dbContext) : IRequestHandler<PlayerInfoQuery, Dictionary<int, PlayerInfo>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<Dictionary<int, PlayerInfo>> Handle(PlayerInfoQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Players
                .Where(x => request.Ids.Contains(x.PlayerId))
                .Select(x => new
                {
                    x.PlayerId,
                    x.AllianceId,
                    x.Name
                })
                .ToDictionaryAsync(x => x.PlayerId, x => new PlayerInfo(x.AllianceId, x.Name), cancellationToken: cancellationToken);
        }
    }

    public record PlayerInfo(int AllianceId, string Name);
}