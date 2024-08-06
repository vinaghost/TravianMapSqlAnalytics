using Features.Shared.Dtos;
using Features.Villages.Shared;
using X.PagedList;

namespace WebMVC.Models.ViewModel.Villages
{
    public class IndexViewModel
    {
        public GetVillagesParameters Parameters { get; init; } = new();
        public IPagedList<VillageDto>? Villages { get; init; }
        public Dictionary<int, List<PopulationDto>> Population { get; init; } = [];
    }
}