using Features.Shared.Dtos;

namespace WebMVC.Models.ViewModel.Players
{
    public class IndexViewModel
    {
        public required PlayerDto Player { get; set; }
        public required IList<VillageDto> Villages { get; set; }
        public required IDictionary<int, IList<PopulationDto>> Population { get; set; }
    }
}