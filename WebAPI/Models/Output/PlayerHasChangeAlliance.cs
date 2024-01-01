namespace WebAPI.Models.Output
{
    public record PlayerHasChangeAlliance(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int ChangeAlliance,
        IEnumerable<AllianceHistoryRecord> Alliances);
}