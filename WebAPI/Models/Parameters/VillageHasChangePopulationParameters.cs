namespace WebAPI.Models.Parameters
{
    public record VillageHasChangePopulationParameters : VillageParameters, IChangePopulationFilterParameter
    {
        public int MinChangePopulation { get; set; } = DefaultParameters.MinChangePopulation;
        public int MaxChangePopulation { get; set; } = DefaultParameters.MaxChangePopulation;
        public DateOnly Date { get; set; } = DefaultParameters.Date;
        public override string Key => $"{base.Key}_{MinChangePopulation}_{MaxChangePopulation}_{Date}";
    }
}