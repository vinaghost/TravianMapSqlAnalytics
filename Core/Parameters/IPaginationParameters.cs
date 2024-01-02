namespace Core.Parameters
{
    public interface IPaginationParameters
    {
        int PageNumber { get; }
        int PageSize { get; }
    }
}