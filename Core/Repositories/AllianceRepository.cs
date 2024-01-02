using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories
{
    public class AllianceRepository(ServerDbContext dbContext) : IAllianceRepository
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<Dictionary<int, AllianceRecord>> GetRecords(List<int> alliancesId, CancellationToken cancellationToken)
        {
            var ids = alliancesId.Distinct().Order().ToList();
            return await _dbContext.Alliances
                .Where(x => ids.Contains(x.AllianceId))
                .ToDictionaryAsync(x => x.AllianceId, x => new AllianceRecord(x.Name), cancellationToken: cancellationToken);
        }
    }
}