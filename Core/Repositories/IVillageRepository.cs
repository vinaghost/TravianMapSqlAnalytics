using Core.Dtos;
using Core.Models;
using Core.Parameters;

namespace Core.Repositories
{
    public interface IVillageRepository
    {
        IEnumerable<VillageDto> GetVillages(IList<int> villageIds);

        Task<List<int>> GetVillageIds(IVillageFilterParameter parameters, CancellationToken cancellationToken);

        Task<Dictionary<int, VillageInfo>> GetVillages(IList<int> villageIds, VillageContainsDistanceParameters parameters, CancellationToken cancellationToken);

        Task<Dictionary<int, VillagePopulationHistory>> GetVillages(IList<int> villageIds, VillageContainsPopulationHistoryParameters parameters, CancellationToken cancellationToken);
    }
}