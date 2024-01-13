using Core.Models;

namespace Core.Repositories
{
    public interface IAllianceRepository
    {
        Task<Dictionary<int, AllianceRecord>> GetRecords(IList<int> alliancesId, CancellationToken cancellationToken);

        Task<Dictionary<int, AllianceRecord>> GetRecords(IList<int> playerIds, DateOnly historyDate, CancellationToken cancellationToken);
    }
}