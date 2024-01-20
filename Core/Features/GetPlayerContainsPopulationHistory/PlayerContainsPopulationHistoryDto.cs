using Core.Features.Shared.Models;

namespace Core.Features.GetPlayerContainsPopulationHistory
{
    public record PlayerContainsPopulationHistoryDto(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int ChangePopulation,
        IList<PopulationHistoryRecord> Populations);
}