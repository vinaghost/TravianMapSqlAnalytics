namespace Core.Features.GetPlayerContainsAllianceHistory
{
    public record AllianceHistoryRecord(
       int AllianceId,
       string AllianceName,
       DateTime Date);
}