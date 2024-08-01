using Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Features.Alliances
{
    public record AllianceDto(int AllianceId, string AllianceName, int PlayerCount);
    public record GetAllianceQuery(int AllianceId) : ICachedQuery<AllianceDto?>
    {
        public string CacheKey => $"{nameof(GetAllianceQuery)}_{AllianceId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetAllianceInfoQueryHandler(VillageDbContext DbContext) : IRequestHandler<GetAllianceQuery, AllianceDto?>
    {
        private readonly VillageDbContext _dbContext = DbContext;

        public async Task<AllianceDto?> Handle(GetAllianceQuery request, CancellationToken cancellationToken)
        {
            var allianceId = request.AllianceId;
            var alliance = await _dbContext.Alliances
                .Where(x => x.Id == allianceId)
                .Select(x => new AllianceDto(
                        x.Id,
                        x.Name,
                        x.PlayerCount
                    ))
                .FirstOrDefaultAsync(cancellationToken);
            return alliance;
        }
    }
}