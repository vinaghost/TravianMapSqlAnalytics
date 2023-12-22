namespace WebAPI.Models.Output
{
    public record Server(string Url, string Region, DateTime StartDate, int AllianceCount, int PlayerCount, int VillageCount);
}