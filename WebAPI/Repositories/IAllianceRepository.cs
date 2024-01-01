using WebAPI.Models.Output;

namespace WebAPI.Repositories
{
    public interface IAllianceRepository
    {
        Task<Dictionary<int, AllianceRecord>> GetRecords(List<int> alliancesId, CancellationToken cancellationToken);
    }
}