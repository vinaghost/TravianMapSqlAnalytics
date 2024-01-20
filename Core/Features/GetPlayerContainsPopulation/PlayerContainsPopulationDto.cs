namespace Core.Features.GetPlayerContainsPopulation
{
    public record PlayerContainsPopulationDto(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int VillageCount,
        int Population);
}