using Features.Shared.Dtos;

namespace WebMVC.Models.ViewModel.Players
{
    public class IndexViewModel
    {
        public PlayerDto? Player { get; set; }
        public IList<Features.Villages.ByDistance.VillageDto> Villages { get; set; } = [];
        public Dictionary<int, List<PopulationDto>> Population { get; set; } = [];
    }
}