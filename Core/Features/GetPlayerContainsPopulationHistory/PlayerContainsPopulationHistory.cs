using Core.Features.Shared.Models;

namespace Core.Features.GetPlayerContainsPopulationHistory
{
    public record PlayerContainsPopulationHistory(
        int AllianceId,
        int PlayerId,
        string PlayerName,
        int ChangePopulation,
        IList<PopulationHistoryRecord> Populations);
}