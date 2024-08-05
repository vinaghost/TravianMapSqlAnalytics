using Features.Shared.Dtos;
using Features.Villages;
using X.PagedList;

namespace WebMVC.Models.ViewModel.Villages
{
    public class IndexViewModel
    {
        public GetVillagesParameters? Parameters { get; set; }
        public IPagedList<VillageDto>? Villages { get; set; }
        public Dictionary<int, List<PopulationDto>>? Population { get; set; }
    }
}