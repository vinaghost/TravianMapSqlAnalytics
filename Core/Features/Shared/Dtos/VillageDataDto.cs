namespace Core.Features.Shared.Dtos
{
    public record VillageDataDto(double Distance, PlayerDto Player, VillageDto Village, IList<PopulationDto> Populations);
}