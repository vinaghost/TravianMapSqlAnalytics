namespace Core.Parameters
{
    public class ServerParameters : IPaginationParameters
    {
        public int PageNumber => DefaultParameters.PageNumber;

        public int PageSize => DefaultParameters.PageSize;

        public string Key => $"{PageNumber}_{PageSize}";
    }
}