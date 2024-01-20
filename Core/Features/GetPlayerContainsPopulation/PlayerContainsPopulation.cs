namespace Core.Features.GetPlayerContainsPopulation
{
    public record PlayerContainsPopulation(
        int AllianceId,
        int PlayerId,
        string PlayerName,
        int VillageCount,
        int Population);
}