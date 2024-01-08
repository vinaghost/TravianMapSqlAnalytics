namespace Core.Models
{
    public record PlayerContainsPopulation(
        int AllianceId,
        int PlayerId,
        string PlayerName,
        int VillageCount,
        int Population);
}