using Features.Shared.Dtos;
using Features.Villages;
using X.PagedList;

namespace WebMVC.Models.ViewModel.Villages
{
    public class InactiveViewModel : BaseViewModel<InactiveParameters, IPagedList<VillageDto>>;
}