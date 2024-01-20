using Core.Features.Shared.Models;

namespace Core.Features.GetPlayerContainsPopulationHistory
{
    public record PlayerPopulationHistory(int ChangePopulation, IList<PopulationHistoryRecord> Populations);
}