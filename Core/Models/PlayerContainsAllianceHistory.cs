namespace Core.Models
{
    public record PlayerContainsAllianceHistory(
        int AllianceId,
        int PlayerId,
        string PlayerName,
        int ChangeAlliance,
        IList<AllianceHistoryRecord> Alliances);
}