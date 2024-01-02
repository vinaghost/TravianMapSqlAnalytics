namespace Core.Models
{
    public record PlayerContainsAllianceHistory(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int ChangeAlliance,
        IEnumerable<AllianceHistoryRecord> Alliances);
}