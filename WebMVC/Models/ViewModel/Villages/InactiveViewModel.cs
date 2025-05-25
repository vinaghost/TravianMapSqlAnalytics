using Features.Queries.Villages;
using Features.Shared.Dtos;
using X.PagedList;

namespace WebMVC.Models.ViewModel.Villages
{
    public class InactiveViewModel
    {
        public GetInactiveVillagesParameters Parameters { get; init; } = new();
        public IPagedList<Features.Shared.Dtos.VillageDto>? Villages { get; init; }
        public Dictionary<int, List<PopulationDto>> Population { get; init; } = [];
    }
}