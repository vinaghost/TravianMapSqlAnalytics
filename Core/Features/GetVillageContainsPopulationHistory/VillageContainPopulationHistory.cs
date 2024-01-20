using Core.Features.Shared.Models;

namespace Core.Features.GetVillageContainsPopulationHistory
{
    public record VillageContainPopulationHistory(
        int PlayerId,
        int VillageId,
        string VillageName,
        int X,
        int Y,
        bool IsCapital,
        int Tribe,
        double Distance,
        int ChangePopulation,
        IEnumerable<PopulationHistoryRecord> Populations);
}