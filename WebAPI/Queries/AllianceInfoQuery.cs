using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Queries
{
    public record AllianceInfoQuery(IEnumerable<int> Ids) : IRequest<Dictionary<int, string>>;

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