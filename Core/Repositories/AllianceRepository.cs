using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Repositories
{
    public class AllianceRepository(ServerDbContext dbContext) : IAllianceRepository
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<Dictionary<int, AllianceRecord>> GetRecords(IList<int> playerIds, CancellationToken cancellationToken)
        {
            var ids = playerIds.Distinct().Order().ToList();

            var allianceIds = await _dbContext.Players
                .Where(x => ids.Contains(x.PlayerId))
                .Select(x => x.AllianceId)
                .Distinct()
                .Order()
                .ToListAsync(cancellationToken);

            return await _dbContext.Alliances
                .Where(x => allianceIds.Contains(x.AllianceId))
                .ToDictionaryAsync(x => x.AllianceId, x => new AllianceRecord(x.Name), cancellationToken);
        }

        public async Task<Dictionary<int, AllianceRecord>> GetRecords(IList<int> playerIds, DateOnly historyDate, CancellationToken cancellationToken)
        {
            var ids = playerIds.Distinct().Order().ToList();
            var date = historyDate.ToDateTime(TimeOnly.MinValue);

            var allianceCurrentIds = _dbContext.Players
                .Where(x => ids.Contains(x.PlayerId))
                .Select(x => x.AllianceId);

            var allianceHistoryIds = _dbContext.PlayersAlliances
                .Where(x => ids.Contains(x.PlayerId))
                .Where(x => x.Date >= date)
                .Select(x => x.AllianceId);

            var allianceIds = await allianceCurrentIds
                .Union(allianceHistoryIds)
                .Distinct()
                .Order()
                .ToListAsync(cancellationToken);

            return await _dbContext.Alliances
                .Where(x => allianceIds.Contains(x.AllianceId))
                .ToDictionaryAsync(x => x.AllianceId, x => new AllianceRecord(x.Name), cancellationToken);
        }
    }
}