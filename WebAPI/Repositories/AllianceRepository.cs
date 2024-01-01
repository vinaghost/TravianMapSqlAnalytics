using Core;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models.Output;

namespace WebAPI.Repositories
{
    public class AllianceRepository(ServerDbContext dbContext) : IAllianceRepository
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<Dictionary<int, AllianceRecord>> GetRecords(List<int> alliancesId, CancellationToken cancellationToken)
        {
            return await _dbContext.Alliances
                .Where(x => alliancesId.Contains(x.AllianceId))
                .ToDictionaryAsync(x => x.AllianceId, x => new AllianceRecord(x.Name), cancellationToken: cancellationToken);
        }
    }
}