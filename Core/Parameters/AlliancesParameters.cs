namespace Core.Parameters
{
    public class AlliancesParameters : IPaginationParameters
    {
        public int PageNumber { get; set; } = DefaultParameters.PageNumber;
        public int PageSize { get; set; } = DefaultParameters.PageSize;
    }
}