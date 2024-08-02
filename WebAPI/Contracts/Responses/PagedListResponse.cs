using X.PagedList;

namespace WebAPI.Contracts.Responses
{
    public class PagedListResponse<T>(IPagedList<T> pagedList)
    {
        public IList<T> Data { get; } = [.. pagedList];
        public int TotalItemCount { get; } = pagedList.TotalItemCount;
        public int PageNumber { get; } = pagedList.PageNumber;
        public int PageSize { get; } = pagedList.PageSize;
        public int PageCount { get; } = pagedList.PageCount;
    }
}