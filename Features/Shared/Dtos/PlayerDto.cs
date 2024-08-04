namespace Features.Shared.Dtos
{
    public record PlayerDto(int AllianceId,
                            int PlayerId,
                            string PlayerName,
                            int VillageCount,
                            int Population);
}