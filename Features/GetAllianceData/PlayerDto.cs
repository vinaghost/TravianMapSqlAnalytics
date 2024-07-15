using Features.Shared.Dtos;

namespace Features.GetAllianceData
{
    public record PlayerDto(int PlayerId, string PlayerName, int VillageCount, IList<PopulationDto> Populations);
}