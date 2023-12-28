using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Queries
{
    public record PlayerNameQuery(IEnumerable<int> Ids) : IRequest<Dictionary<int, PlayerInfo>>;

    public class PlayerNameQueryHandler(ServerDbContext dbContext) : IRequestHandler<PlayerNameQuery, Dictionary<int, PlayerInfo>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<Dictionary<int, PlayerInfo>> Handle(PlayerNameQuery request, CancellationToken cancellationToken)
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