using WebAPI.Models.Output;
using WebAPI.Models.Parameters;

namespace WebAPI.Repositories
{
    public interface IVillageRepository
    {
        IEnumerable<VillageContainsDistance> GetVillages(VillageParameters parameters);

        IEnumerable<VillageContainPopulationHistory> GetVillages(VillageContainsPopulationHistoryParameters parameters);
    }
}