namespace MapSqlQuery.Models.Input
{
    public interface IPagingInput
    {
        int PageNumber { get; set; }
        int PageSize { get; set; }
    }
}