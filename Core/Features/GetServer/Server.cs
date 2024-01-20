namespace Core.Features.GetServer
{
    public record Server(string Url, string Region, DateTime StartDate, int AllianceCount, int PlayerCount, int VillageCount);
}