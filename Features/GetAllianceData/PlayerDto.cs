using Features.Shared.Dtos;

namespace Features.GetAllianceData
{
    public record PlayerDto(int PlayerId, string PlayerName, int Population, int VillageCount, IList<PopulationDto> Populations);
}