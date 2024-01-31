using Core.Features.Shared.Dtos;

namespace Core.Features.GetInactiveVillage
{
    public class InactiveVillageDto
    {
        public required double Distance { get; set; }

        public required PlayerDto Player { get; set; }

        public required VillageDto Village { get; set; }
        public List<PopulationDto> Populations { get; set; }
    }
}