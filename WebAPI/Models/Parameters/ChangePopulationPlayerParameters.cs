namespace WebAPI.Models.Parameters
{
    public class ChangePopulationPlayerParameters : PlayerParameters, IChangePopulationFilterParameter
    {
        public int MinChangePopulation { get; } = DefaultParameters.MinChangePopulation;
        public int MaxChangePopulation { get; } = DefaultParameters.MaxChangePopulation;
        public DateOnly Date { get; } = DefaultParameters.Date;
    }
}