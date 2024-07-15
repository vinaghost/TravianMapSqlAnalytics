using Features.GetInactiveFarms;
using Features.Shared.Dtos;
using X.PagedList;

namespace WebMVC.ViewModels.Villages
{
    public class InactiveFarmViewModel : BaseViewModel<InactiveFarmParameters, IPagedList<VillageDataDto>>;
}