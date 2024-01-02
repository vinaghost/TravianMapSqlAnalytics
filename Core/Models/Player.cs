namespace Core.Models
{
    public record Player(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int VillageCount,
        int Population);
}