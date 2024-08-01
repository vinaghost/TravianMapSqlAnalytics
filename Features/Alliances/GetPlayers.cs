using Features.Players;
using Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Features.Alliances
{
    public record GetPlayersQuery(int AllianceId) : ICachedQuery<List<PlayerDto>>
    {
        public string CacheKey => $"{nameof(GetPlayersQuery)}_{AllianceId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetPlayersQueryHandler(VillageDbContext DbContext) : IRequestHandler<GetPlayersQuery, List<PlayerDto>>
    {
        private readonly VillageDbContext _dbContext = DbContext;

        public async Task<List<PlayerDto>> Handle(GetPlayersQuery request, CancellationToken cancellationToken)
        {
            var allianceId = request.AllianceId;
            var players = await _dbContext.Players
                 .Where(x => x.AllianceId == allianceId)
                 .OrderByDescending(x => x.Population)
                 .Select(x => new PlayerDto
                     (
                         x.Id,
                         x.Name,
                         x.VillageCount,
                         x.Population
                     ))
                 .ToListAsync(cancellationToken);
            return players;
        }
    }
}