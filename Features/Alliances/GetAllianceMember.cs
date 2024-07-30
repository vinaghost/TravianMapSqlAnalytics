using Features.Shared.Dtos;
using Features.Shared.Parameters;
using Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Features.Alliances
{
    public record PlayerDto(int PlayerId, string PlayerName, int VillageCount, IList<PopulationDto> Populations);
    public record GetAllianceMemberQuery(int AllianceId) : ICachedQuery<List<PlayerDto>>
    {
        public string CacheKey => $"{nameof(GetAllianceMemberQuery)}_{AllianceId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetAllianceMemberQueryHandler(VillageDbContext DbContext) : IRequestHandler<GetAllianceMemberQuery, List<PlayerDto>>
    {
        private readonly VillageDbContext _dbContext = DbContext;

        public async Task<List<PlayerDto>> Handle(GetAllianceMemberQuery request, CancellationToken cancellationToken)
        {
            var allianceId = request.AllianceId;
            var members = await _dbContext.Players
                 .Where(x => x.AllianceId == allianceId)
                 .OrderByDescending(x => x.Population)
                 .GroupJoin(_dbContext.PlayersHistory.Where(x => x.Date >= EF.Constant(DefaultParameters.Date)),
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
            return members;
        }
    }
}