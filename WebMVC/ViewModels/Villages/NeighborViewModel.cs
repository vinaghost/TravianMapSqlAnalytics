using Features.GetNeighbors;
using Features.Shared.Dtos;
using X.PagedList;

namespace WebMVC.ViewModels.Villages
{
    public class NeighborViewModel : BaseViewModel<NeighborsParameters, IPagedList<VillageDataDto>>;
}