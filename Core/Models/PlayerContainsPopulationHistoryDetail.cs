namespace Core.Models
{
    public record PlayerContainsPopulationHistoryDetail(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int ChangePopulation,
        IList<PopulationHistoryRecord> Populations);
}