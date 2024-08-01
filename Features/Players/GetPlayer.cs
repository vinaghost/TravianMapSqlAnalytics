using Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Features.Players
{
    public record PlayerDto(int PlayerId, string PlayerName, int VillageCount, int Population);
    public record GetPlayerQuery(int PlayerId) : ICachedQuery<PlayerDto?>
    {
        public string CacheKey => $"{nameof(GetPlayerQuery)}_{PlayerId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetPlayerQueryHandler(VillageDbContext DbContext) : IRequestHandler<GetPlayerQuery, PlayerDto?>
    {
        private readonly VillageDbContext _dbContext = DbContext;

        public async Task<PlayerDto?> Handle(GetPlayerQuery request, CancellationToken cancellationToken)
        {
            var PlayerId = request.PlayerId;
            var Player = await _dbContext.Players
                .Where(x => x.Id == PlayerId)
                .Select(x => new PlayerDto(
                        x.Id,
                        x.Name,
                        x.VillageCount,
                        x.Population

                    ))
                .FirstOrDefaultAsync(cancellationToken);
            return Player;
        }
    }
}