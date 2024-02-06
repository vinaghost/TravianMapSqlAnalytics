using Core.Features.Shared.Dtos;

namespace Core.Features.GetInactiveFarms
{
    public class InactiveFarmDto
    {
        public required double Distance { get; set; }

        public required PlayerDto Player { get; set; }

        public required VillageDto Village { get; set; }
        public List<PopulationDto> Populations { get; set; }
    }
}