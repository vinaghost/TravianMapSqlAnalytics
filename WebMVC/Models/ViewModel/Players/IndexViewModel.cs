using Features.Shared.Dtos;

namespace WebMVC.Models.ViewModel.Players
{
    public class IndexViewModel
    {
        public PlayerDto? Player { get; set; }
        public IList<VillageDto> Villages { get; set; } = [];
        public Dictionary<int, List<PopulationDto>> Population { get; set; } = [];
    }
}