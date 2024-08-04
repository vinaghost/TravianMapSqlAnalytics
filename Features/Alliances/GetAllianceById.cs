using Features.Shared.Dtos;
using Features.Shared.Query;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Features.Alliances
{
    public record GetAllianceByIdQuery(int AllianceId) : ICachedQuery<AllianceDto?>
    {
        public string CacheKey => $"{nameof(GetAllianceByIdQuery)}_{AllianceId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetAllianceByIdQueryHandler(VillageDbContext DbContext) : IRequestHandler<GetAllianceByIdQuery, AllianceDto?>
    {
        private readonly VillageDbContext _dbContext = DbContext;

        public async Task<AllianceDto?> Handle(GetAllianceByIdQuery request, CancellationToken cancellationToken)
        {
            var allianceId = request.AllianceId;
            var alliance = await _dbContext.Alliances
                .Where(x => x.Id == allianceId)
                .Select(x => new AllianceDto(
                        x.Id,
                        string.IsNullOrEmpty(x.Name) ? "No alliance" : x.Name,
                        x.PlayerCount
                    ))
                .FirstOrDefaultAsync(cancellationToken);
            return alliance;
        }
    }
}