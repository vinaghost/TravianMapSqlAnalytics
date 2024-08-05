using Features.Shared.Dtos;
using X.PagedList;

namespace WebMVC.Models.ViewModel
{
    public class BaseViewModel<TParameters, TData>
    {
        public required TParameters Parameters { get; set; }
        public required IPagedList<TData> Data { get; set; }
        public required IDictionary<int, IList<PopulationDto>> Population { get; set; }
    }
}