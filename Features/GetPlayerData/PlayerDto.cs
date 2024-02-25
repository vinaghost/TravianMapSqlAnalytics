namespace Features.GetPlayerData
{
    public record PlayerDto(int PlayerId, string PlayerName, int AllianceId, string AllianceName, int Population, int VillageCount);
}