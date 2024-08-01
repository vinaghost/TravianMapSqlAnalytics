using Features.Shared.Dtos;
using Features.Shared.Parameters;
using Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Features.Players
{
    public record VillageDto(int MapId, string VillageName, int X, int Y, int Population, bool IsCapital, int Tribe, IList<PopulationDto> Populations);
    public record GetVillagesQuery(int PlayerId) : ICachedQuery<List<VillageDto>>
    {
        public string CacheKey => $"{nameof(GetVillagesQuery)}_{PlayerId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetVillageQueryHandler(VillageDbContext DbContext) : IRequestHandler<GetVillagesQuery, List<VillageDto>>
    {
        private readonly VillageDbContext _dbContext = DbContext;

        public async Task<List<VillageDto>> Handle(GetVillagesQuery request, CancellationToken cancellationToken)
        {
            var playerId = request.PlayerId;
            var villages = await _dbContext.Villages
                .Where(x => x.PlayerId == playerId)
                .OrderByDescending(x => x.Population)
                .GroupJoin(_dbContext.VillagesHistory.Where(x => x.Date >= DefaultParameters.Date),
                    x => x.Id,
                    x => x.VillageId,
                    (village, populations) => new VillageDto
                    (
                        village.MapId,
                        village.Name,
                        village.X,
                        village.Y,
                        village.Population,
                        village.IsCapital,
                        village.Tribe,
                        populations
                            .OrderByDescending(x => x.Date)
                            .Select(x => new PopulationDto(x.Date, x.Population, x.ChangePopulation))
                            .ToList()
                    ))
                .ToListAsync(cancellationToken);
            return villages;
        }
    }
}