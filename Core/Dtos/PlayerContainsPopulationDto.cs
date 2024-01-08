namespace Core.Dtos
{
    public record PlayerContainsPopulationDto(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int VillageCount,
        int Population);
}