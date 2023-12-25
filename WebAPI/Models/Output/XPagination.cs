using System.Text.Json;
using X.PagedList;

namespace WebAPI.Models.Output
{
    public record XPagination(int TotalItemCount, int PageSize, int PageNumber, int PageCount, bool HasNextPage, bool HasPreviousPage);

    public static class XPaginationExtension
    {
        public static XPagination ToXpagination(this IPagedList pagedList)
        {
            return new XPagination(
                pagedList.TotalItemCount,
                pagedList.PageSize,
                pagedList.PageNumber,
                pagedList.PageCount,
                pagedList.HasNextPage,
                pagedList.HasPreviousPage);
        }

        public static string ToJson(this XPagination xPagination)
        {
            return JsonSerializer.Serialize(xPagination);
        }
    }
}