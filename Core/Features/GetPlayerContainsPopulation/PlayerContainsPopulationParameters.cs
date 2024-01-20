using Core.Features.Shared.Parameters;

namespace Core.Features.GetPlayerContainsPopulation
{
    public class PlayerContainsPopulationParameters : IPaginationParameters, IPlayerFilterParameter, ISortParameters
    {
        public int PageNumber { get; set; } = DefaultParameters.PageNumber;
        public int PageSize { get; set; } = DefaultParameters.PageSize;

        public List<int> Alliances { get; set; } = [];
        public List<int> Players { get; set; } = [];

        public string Key => $"{PageNumber}_{PageSize}_{string.Join(',', Alliances)}_{string.Join(',', Players)}_{SortOrder}_{SortField}";

        public int SortOrder { get; set; } = DefaultParameters.SortOrder;

        public string SortField { get; set; } = DefaultParameters.SortField;
    }
}