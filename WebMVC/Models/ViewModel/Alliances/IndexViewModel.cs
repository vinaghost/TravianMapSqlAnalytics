using Features.Alliances;
using Features.Players;
using Features.Populations;

namespace WebMVC.Models.ViewModel.Alliances
{
    public class IndexViewModel
    {
        public AllianceDto? Alliance { get; set; }
        public IList<PlayerDto> Players { get; set; } = [];
        public Dictionary<int, List<PopulationDto>> Population { get; set; } = [];
    }
}