using Core.Models;

namespace Core.Repositories
{
    public interface IAllianceRepository
    {
        Task<Dictionary<int, AllianceRecord>> GetRecords(List<int> alliancesId, CancellationToken cancellationToken);
    }
}