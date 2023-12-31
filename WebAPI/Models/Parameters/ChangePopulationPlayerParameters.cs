namespace WebAPI.Models.Parameters
{
    public class ChangePopulationPlayerParameters : PlayerParameters, IChangePopulationFilterParameter
    {
        public int MinChangePopulation { get; set; } = DefaultParameters.MinChangePopulation;
        public int MaxChangePopulation { get; set; } = DefaultParameters.MaxChangePopulation;
        public DateOnly Date { get; set; } = DefaultParameters.Date;
    }
}