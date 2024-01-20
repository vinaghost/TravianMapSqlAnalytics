namespace Core.Features.GetPlayerContainsAllianceHistory
{
    public record PlayerContainsAllianceHistoryDto(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int ChangeAlliance,
        IList<AllianceHistoryRecord> Alliances);
}