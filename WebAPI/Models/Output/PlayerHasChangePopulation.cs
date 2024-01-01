namespace WebAPI.Models.Output
{
    public record PlayerHasChangePopulation(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int ChangePopulation,
        IEnumerable<PopulationHistoryRecord> Populations);
}