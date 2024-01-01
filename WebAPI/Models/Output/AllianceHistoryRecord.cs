namespace WebAPI.Models.Output
{
    public record AllianceHistoryRecord(
       int AllianceId,
       string AllianceName,
       DateTime Date);
}