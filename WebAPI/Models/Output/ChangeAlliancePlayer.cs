namespace WebAPI.Models.Output
{
    public record ChangeAlliancePlayer(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int ChangeAlliance,
        IEnumerable<AllianceHistoryRecord> Alliances);

    public record AllianceHistoryRecord(
       int AllianceId,
       string AllianceName,
       DateTime Date);
}