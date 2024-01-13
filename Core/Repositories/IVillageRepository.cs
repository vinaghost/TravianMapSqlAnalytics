using Core.Entities;
using Core.Models;
using Core.Parameters;

namespace Core.Repositories
{
    public interface IVillageRepository
    {
        IQueryable<Village> GetBaseQueryable(IVillageFilterParameter parameters);
        IEnumerable<VillageContainsDistance> GetVillages(VillageContainsDistanceParameters parameters);

        IEnumerable<VillageContainPopulationHistory> GetVillages(VillageContainsPopulationHistoryParameters parameters);
    }
}