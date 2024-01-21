using Core.Features.Shared.Parameters;

namespace Core.Features.GetPlayerContainsAllianceHistory
{
    public class PlayerContainsAllianceHistoryParameters : IPaginationParameters, IPlayerFilterParameters, IAllianceHistoryFilterParameters, ISortParameters
    {
        public int PageNumber { get; set; } = DefaultParameters.PageNumber;
        public int PageSize { get; set; } = DefaultParameters.PageSize;

        public List<int> Alliances { get; set; } = [];
        public List<int> Players { get; set; } = [];
        public DateTime Date { get; set; } = DefaultParameters.Date;
        public int MinChangeAlliance { get; set; } = DefaultParameters.MinChangeAlliance;
        public int MaxChangeAlliance { get; set; } = DefaultParameters.MaxChangeAlliance;

        public int SortOrder { get; set; } = DefaultParameters.SortOrder;

        public string SortField { get; set; } = DefaultParameters.SortField;
        public string Key => $"{PageNumber}_{PageSize}_{string.Join(',', Alliances.Distinct().Order())}_{string.Join(',', Players.Distinct().Order())}_{MinChangeAlliance}_{MaxChangeAlliance}_{Date}_{SortOrder}_{SortField}";
    }
}