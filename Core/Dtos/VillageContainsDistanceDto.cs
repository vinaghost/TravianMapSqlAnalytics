namespace Core.Dtos
{
    public record VillageContainsDistanceDto(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int VillageId,
        string VillageName,
        int X,
        int Y,
        int Population,
        bool IsCapital,
        int Tribe,
        double Distance);
}