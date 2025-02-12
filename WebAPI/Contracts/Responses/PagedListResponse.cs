using X.PagedList;

namespace WebAPI.Contracts.Responses
{
    public class PagedListResponse<T>(IPagedList<T> pagedList)
    {
        public IList<T> Data { get; } = [.. pagedList];
        public PagianationData Pagianation { get; } = new(pagedList);

        public class PagianationData(IPagedList pagedList)
        {
            public int TotalItemCount { get; } = pagedList.TotalItemCount;
            public int PageNumber { get; } = pagedList.PageNumber;
            public int PageSize { get; } = pagedList.PageSize;
            public int PageCount { get; } = pagedList.PageCount;
            public bool HasPreviousPage { get; } = pagedList.HasPreviousPage;
            public bool HasNextPage { get; } = pagedList.HasNextPage;
            public bool IsFirstPage { get; } = pagedList.IsFirstPage;
            public bool IsLastPage { get; } = pagedList.IsLastPage;
            public int FirstItemOnPage { get; } = pagedList.FirstItemOnPage;
            public int LastItemOnPage { get; } = pagedList.LastItemOnPage;
        }
    }
}