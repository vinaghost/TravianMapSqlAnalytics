using Features.Shared.Dtos;
using Features.Shared.Query;
using Microsoft.EntityFrameworkCore;

namespace Features.Players
{
    public record GetPlayerByIdQuery(int PlayerId) : ICachedQuery<PlayerDto?>
    {
        public string CacheKey => $"{nameof(GetPlayerByIdQuery)}_{PlayerId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetPlayerByIdQueryHandler(VillageDbContext DbContext) : IRequestHandler<GetPlayerByIdQuery, PlayerDto?>
    {
        private readonly VillageDbContext _dbContext = DbContext;

        public async Task<PlayerDto?> Handle(GetPlayerByIdQuery request, CancellationToken cancellationToken)
        {
            var PlayerId = request.PlayerId;
            var Player = await _dbContext.Players
                .Where(x => x.Id == PlayerId)
                .Join(_dbContext.Alliances,
                    x => x.AllianceId,
                    x => x.Id,
                    (player, alliance) => new PlayerDto(
                        alliance.Id,
                        alliance.Name,
                        player.Id,
                        player.Name,
                        player.VillageCount,
                        player.Population
                    ))
                .FirstOrDefaultAsync(cancellationToken);
            return Player;
        }
    }
}