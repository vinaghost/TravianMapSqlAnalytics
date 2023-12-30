namespace WebAPI.Models.Output
{
    public record ChangeAlliancePlayer(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int ChangeAlliance,
        IEnumerable<AllianceRecord> Alliances);

    public record AllianceRecord(
       int AllianceId,
       string AllianceName,
       DateTime date);
}