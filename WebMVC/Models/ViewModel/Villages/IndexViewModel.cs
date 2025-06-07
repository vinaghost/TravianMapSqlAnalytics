using Features.Populations;
using Features.Villages;
using Features.Villages.GetVillages;
using X.PagedList;

namespace WebMVC.Models.ViewModel.Villages
{
    public class IndexViewModel
    {
        public GetVillagesParameters Parameters { get; init; } = new();
        public IPagedList<DetailVillageDto>? Villages { get; init; }
        public Dictionary<int, List<PopulationDto>> Population { get; init; } = [];
    }
}