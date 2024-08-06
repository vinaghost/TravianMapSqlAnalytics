using Features.Shared.Dtos;
using Features.Villages;
using X.PagedList;

namespace WebMVC.Models.ViewModel.Villages
{
    public class InactiveViewModel
    {
        public GetInactiveParameters Parameters { get; init; } = new();
        public IPagedList<VillageDto>? Villages { get; init; }
        public Dictionary<int, List<PopulationDto>> Population { get; init; } = [];
    }
}