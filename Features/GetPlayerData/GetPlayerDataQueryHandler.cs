using Features.Shared.Dtos;
using Features.Shared.Parameters;

using MediatR;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Features.GetPlayerData
{
    public class GetPlayerDataQueryHandler(VillageDbContext DbContext) : IRequestHandler<GetPlayerDataQuery, PlayerDataDto?>
    {
        private readonly VillageDbContext _dbContext = DbContext;

        public async Task<PlayerDataDto?> Handle(GetPlayerDataQuery request, CancellationToken cancellationToken)
        {
            var playerId = request.PlayerId;
            var playerData = await _dbContext.Players
                .Where(x => x.Id == playerId)
                .Join(_dbContext.Alliances,
                    x => x.AllianceId,
                    x => x.Id,
                    (player, alliance) => new PlayerDto(
                        player.Id,
                        player.Name,
                        alliance.Id,
                        alliance.Name,
                        player.Population,
                        player.VillageCount
                    ))
                .FirstOrDefaultAsync(cancellationToken);

            if (playerData is null) return null;

            var villageData = await _dbContext.Villages
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

            return new PlayerDataDto(playerData, villageData);
        }
    }
}