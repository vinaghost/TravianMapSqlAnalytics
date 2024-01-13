namespace Core.Models
{
    public record PlayerAllianceHistory(int ChangeAlliance, IList<AllianceHistoryRecord> Alliances);
}