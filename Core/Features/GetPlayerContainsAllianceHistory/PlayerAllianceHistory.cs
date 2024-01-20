namespace Core.Features.GetPlayerContainsAllianceHistory
{
    public record PlayerAllianceHistory(int ChangeAlliance, IList<AllianceHistoryRecord> Alliances);
}