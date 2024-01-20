using Core.Features.Shared.Parameters;

namespace Core.Features.GetPlayerContainsPopulationHistory
{
    public class PlayerContainsPopulationHistoryParameters : IPaginationParameters, IPlayerFilterParameter, IPopulationHistoryFilterParameter, ISortParameters
    {
        public int PageNumber { get; set; } = DefaultParameters.PageNumber;
        public int PageSize { get; set; } = DefaultParameters.PageSize;

        public List<int> Alliances { get; set; } = [];
        public List<int> Players { get; set; } = [];

        public int MinChangePopulation { get; set; } = DefaultParameters.MinChangePopulation;
        public int MaxChangePopulation { get; set; } = DefaultParameters.MaxChangePopulation;

        public DateOnly Date { get; set; } = DefaultParameters.Date;

        public int SortOrder { get; set; } = DefaultParameters.SortOrder;

        public string SortField { get; set; } = DefaultParameters.SortField;

        public string Key => $"{PageNumber}_{PageSize}_{string.Join(',', Alliances)}_{string.Join(',', Players)}_{MinChangePopulation}_{MaxChangePopulation}_{Date}_{SortOrder}_{SortField}";
    }
}