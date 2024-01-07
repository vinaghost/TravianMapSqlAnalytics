namespace Core.Models
{
    public record PlayerContainsAllianceHistoryDetail(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int ChangeAlliance,
        IList<AllianceHistoryRecord> Alliances);
}