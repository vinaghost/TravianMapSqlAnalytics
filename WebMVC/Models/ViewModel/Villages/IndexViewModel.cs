using Features.Dtos;
using Features.Queries.Villages;
using X.PagedList;

namespace WebMVC.Models.ViewModel.Villages
{
    public class IndexViewModel
    {
        public VillagesParameters Parameters { get; init; } = new();
        public IPagedList<DetailVillageDto>? Villages { get; init; }
        public Dictionary<int, List<PopulationDto>> Population { get; init; } = [];
    }
}