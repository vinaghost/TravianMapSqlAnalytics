namespace WebAPI.Models.Parameters
{
    public class AlliancesParameters : IPaginationParameters
    {
        public int PageNumber { get; } = DefaultParameters.PageNumber;
        public int PageSize { get; } = DefaultParameters.PageSize;
    }
}