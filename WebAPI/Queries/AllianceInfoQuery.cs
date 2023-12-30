using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Queries
{
    public record AllianceInfoQuery(List<int> Ids) : IRequest<Dictionary<int, string>>
    {
        private readonly List<int> ids = Ids;
        public List<int> Ids => ids.Distinct().Order().ToList();
        public string CacheKey => $"{nameof(AllianceInfoQuery)}_{string.Join(',', Ids)}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class AllianceInfoQueryHandler(ServerDbContext dbContext) : IRequestHandler<AllianceInfoQuery, Dictionary<int, string>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<Dictionary<int, string>> Handle(AllianceInfoQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext.Alliances
                .Where(x => request.Ids.Contains(x.AllianceId))
                .Select(x => new { x.AllianceId, x.Name })
                .ToDictionaryAsync(x => x.AllianceId, x => x.Name, cancellationToken: cancellationToken);
        }
    }
}