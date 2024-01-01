namespace WebAPI.Models.Parameters
{
    public class PlayerParameters : IPaginationParameters, IPlayerFilterParameter
    {
        public int PageNumber { get; set; } = DefaultParameters.PageNumber;
        public int PageSize { get; set; } = DefaultParameters.PageSize;

        public List<int> Alliances { get; set; } = [];
        public List<int> Players { get; set; } = [];

        public virtual string Key => $"{PageNumber}_{PageSize}_{string.Join(',', Alliances)}_{string.Join(',', Players)}";
    }
}