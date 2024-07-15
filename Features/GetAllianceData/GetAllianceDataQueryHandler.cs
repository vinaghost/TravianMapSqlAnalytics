using Features.Shared.Dtos;
using Features.Shared.Parameters;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Features.GetAllianceData
{
    public class GetAllianceDataQueryHandler(VillageDbContext DbContext) : IRequestHandler<GetAllianceDataQuery, AllianceDataDto?>
    {
        private readonly VillageDbContext _dbContext = DbContext;

        public async Task<AllianceDataDto?> Handle(GetAllianceDataQuery request, CancellationToken cancellationToken)
        {
            var allianceId = request.AllianceId;
            var allianceData = await _dbContext.Alliances
                .Where(x => x.Id == allianceId)
                .Select(x => new AllianceDto(
                        x.Id,
                        x.Name,
                        x.PlayerCount
                    ))
                .FirstOrDefaultAsync(cancellationToken);

            if (allianceData is null) return null;

            var playerData = await _dbContext.Players
                .Where(x => x.AllianceId == allianceId)
                .OrderByDescending(x => x.Population)
                .GroupJoin(_dbContext.PlayersHistory.Where(x => x.Date >= DefaultParameters.Date),
                    x => x.Id,
                    x => x.PlayerId,
                    (player, populations) => new PlayerDto
                    (
                        player.Id,
                        player.Name,
                        player.VillageCount,
                        populations
                            .OrderByDescending(x => x.Date)
                            .Select(x => new PopulationDto(x.Date, x.Population, x.ChangePopulation))
                            .ToList()
                    ))
                .ToListAsync(cancellationToken);

            return new AllianceDataDto(allianceData, playerData);
        }
    }
}