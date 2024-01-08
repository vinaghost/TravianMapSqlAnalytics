using Core.Models;

namespace Core.Dtos
{
    public record PlayerContainsPopulationHistoryDto(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int ChangePopulation,
        IList<PopulationHistoryRecord> Populations);
}