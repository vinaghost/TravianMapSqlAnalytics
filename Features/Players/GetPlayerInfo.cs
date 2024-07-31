using Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Features.Players
{
    public record PlayerDto(int PlayerId, string PlayerName, int VillageCount);
    public record GetPlayerInfoQuery(int PlayerId) : ICachedQuery<PlayerDto?>
    {
        public string CacheKey => $"{nameof(GetPlayerInfoQuery)}_{PlayerId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetPlayerInfoQueryHandler(VillageDbContext DbContext) : IRequestHandler<GetPlayerInfoQuery, PlayerDto?>
    {
        private readonly VillageDbContext _dbContext = DbContext;

        public async Task<PlayerDto?> Handle(GetPlayerInfoQuery request, CancellationToken cancellationToken)
        {
            var PlayerId = request.PlayerId;
            var Player = await _dbContext.Players
                .Where(x => x.Id == PlayerId)
                .Select(x => new PlayerDto(
                        x.Id,
                        x.Name,
                        x.VillageCount
                    ))
                .FirstOrDefaultAsync(cancellationToken);
            return Player;
        }
    }
}