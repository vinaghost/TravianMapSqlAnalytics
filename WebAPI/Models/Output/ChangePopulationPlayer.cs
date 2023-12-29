namespace WebAPI.Models.Output
{
    public record ChangePopulationPlayer(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int ChangePopulation,
        IEnumerable<Population> Populations);
}