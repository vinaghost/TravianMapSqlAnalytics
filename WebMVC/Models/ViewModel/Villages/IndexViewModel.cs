using Features.Shared.Dtos;
using Features.Villages;
using X.PagedList;

namespace WebMVC.Models.ViewModel.Villages
{
    public class IndexViewModel : BaseViewModel<GetVillagesParameters, IPagedList<VillageDto>, IDictionary<int, PopulationDto>>;
}