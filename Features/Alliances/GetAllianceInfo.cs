using Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Features.Alliances
{
    public record AllianceDto(int AllianceId, string AllianceName, int PlayerCount);
    public record GetAllianceInfoQuery(int AllianceId) : ICachedQuery<AllianceDto?>
    {
        public string CacheKey => $"{nameof(GetAllianceInfoQuery)}_{AllianceId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetAllianceInfoQueryHandler(VillageDbContext DbContext) : IRequestHandler<GetAllianceInfoQuery, AllianceDto?>
    {
        private readonly VillageDbContext _dbContext = DbContext;

        public async Task<AllianceDto?> Handle(GetAllianceInfoQuery request, CancellationToken cancellationToken)
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