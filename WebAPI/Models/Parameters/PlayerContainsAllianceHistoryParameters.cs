namespace WebAPI.Models.Parameters
{
    public class PlayerContainsAllianceHistoryParameters : PlayerParameters, IAllianceHistoryFilterParameter
    {
        public DateOnly Date { get; set; } = DefaultParameters.Date;
        public int MinChangeAlliance { get; set; } = DefaultParameters.MinChangeAlliance;
        public int MaxChangeAlliance { get; set; } = DefaultParameters.MaxChangeAlliance;
        public override string Key => $"{base.Key}_{MinChangeAlliance}_{MaxChangeAlliance}_{Date}";
    }
}