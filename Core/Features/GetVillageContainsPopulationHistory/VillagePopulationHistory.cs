using Core.Features.Shared.Models;

namespace Core.Features.GetVillageContainsPopulationHistory
{
    public record VillagePopulationHistory(int ChangePopulation, IList<PopulationHistoryRecord> Populations);
}