namespace WebAPI.Models.Output
{
    public record VillageContainsDistance(
        int PlayerId,
        int VillageId,
        string VillageName,
        int X,
        int Y,
        int Population,
        bool IsCapital,
        int Tribe,
        double Distance);
}