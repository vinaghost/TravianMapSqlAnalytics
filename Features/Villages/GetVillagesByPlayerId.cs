using Features.Shared.Enums;
using Features.Shared.Query;
using Microsoft.EntityFrameworkCore;

namespace Features.Villages.ByPlayerId
{
    public record VillageDto(int MapId,
                             int VillageId,
                             string VillageName,
                             int X,
                             int Y,
                             Tribe Tribe,
                             int Population,
                             bool IsCapital);
    public record GetVillagesByPlayerIdQuery(int PlayerId) : ICachedQuery<IList<VillageDto>>
    {
        public string CacheKey => $"{nameof(GetVillagesByPlayerIdQuery)}_{PlayerId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetVillagesByPlayerIdQueryHandler(VillageDbContext DbContext) : IRequestHandler<GetVillagesByPlayerIdQuery, IList<VillageDto>>
    {
        private readonly VillageDbContext _dbContext = DbContext;

        public async Task<IList<VillageDto>> Handle(GetVillagesByPlayerIdQuery request, CancellationToken cancellationToken)
        {
            var playerId = request.PlayerId;
            var villages = await _dbContext.Villages
                .Where(x => x.PlayerId == playerId)
                .Select(x => new VillageDto(
                    x.MapId,
                    x.Id,
                    x.Name,
                    x.X,
                    x.Y,
                    (Tribe)x.Tribe,
                    x.Population,
                    x.IsCapital
                ))
                .ToListAsync(cancellationToken);
            return villages;
        }
    }
}