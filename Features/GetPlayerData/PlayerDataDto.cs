namespace Features.GetPlayerData
{
    public record PlayerDataDto(PlayerDto Player, IList<VillageDto> Villages);
}