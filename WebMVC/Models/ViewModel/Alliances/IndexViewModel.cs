using Features.Shared.Dtos;

namespace WebMVC.Models.ViewModel.Alliances
{
    public class IndexViewModel
    {
        public required AllianceDto Alliance { get; set; }
        public required IList<PlayerDto> Players { get; set; }
        public required IDictionary<int, IList<PopulationDto>> Population { get; set; }
    }
}