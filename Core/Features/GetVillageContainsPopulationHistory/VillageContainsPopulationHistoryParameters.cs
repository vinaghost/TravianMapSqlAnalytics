using Core.Features.Shared.Parameters;

namespace Core.Features.GetVillageContainsPopulationHistory
{
    public record VillageContainsPopulationHistoryParameters : IPaginationParameters, IVillageFilterParameters, IPopulationHistoryFilterParameters, ISortParameters
    {
        public int PageNumber { get; set; } = DefaultParameters.PageNumber;
        public int PageSize { get; set; } = DefaultParameters.PageSize;
        public List<int> Alliances { get; set; } = [];
        public List<int> Players { get; set; } = [];

        public int MinPopulation { get; set; } = DefaultParameters.MinPopulation;
        public int MaxPopulation { get; set; } = DefaultParameters.MaxPopulation;

        public int Tribe { get; set; } = DefaultParameters.Tribe;

        public bool IgnoreCapital { get; set; } = DefaultParameters.IgnoreCapital;

        public bool IgnoreNormalVillage { get; set; } = DefaultParameters.IgnoreNormalVillage;
        public string Key => $"{PageNumber}_{PageSize}_{Tribe}_{IgnoreCapital}_{IgnoreNormalVillage}_{MinPopulation}_{MaxPopulation}_{string.Join(',', Alliances.Distinct().Order())}_{string.Join(',', Players.Distinct().Order())}_{MinChangePopulation}_{MaxChangePopulation}_{Date}_{SortOrder}_{SortField}";

        public int MinChangePopulation { get; set; } = DefaultParameters.MinChangePopulation;
        public int MaxChangePopulation { get; set; } = DefaultParameters.MaxChangePopulation;
        public DateTime Date { get; set; } = DefaultParameters.Date;

        public int SortOrder { get; set; } = DefaultParameters.SortOrder;

        public string SortField { get; set; } = DefaultParameters.SortField;
    }
}