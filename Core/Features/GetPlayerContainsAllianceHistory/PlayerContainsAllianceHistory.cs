namespace Core.Features.GetPlayerContainsAllianceHistory
{
    public record PlayerContainsAllianceHistory(
        int AllianceId,
        int PlayerId,
        string PlayerName,
        int ChangeAlliance,
        IList<AllianceHistoryRecord> Alliances);
}