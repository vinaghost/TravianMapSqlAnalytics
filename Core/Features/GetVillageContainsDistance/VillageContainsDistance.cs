namespace Core.Features.GetVillageContainsDistance
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