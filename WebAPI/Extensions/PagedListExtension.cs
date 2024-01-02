using System.Text.Json;
using X.PagedList;

namespace WebAPI.Extensions
{
    public static class PagedListExtension
    {
        private record XPagination(int TotalItemCount, int PageSize, int PageNumber, int PageCount, bool HasNextPage, bool HasPreviousPage);

        public static string ToJson(this IPagedList pagedList)
        {
            return JsonSerializer.Serialize(new XPagination(
                pagedList.TotalItemCount,
                pagedList.PageSize,
                pagedList.PageNumber,
                pagedList.PageCount,
                pagedList.HasNextPage,
                pagedList.HasPreviousPage));
        }
    }
}