namespace WebAPI.Models.Parameters
{
    public class ChangePopulationPlayerParameters : PlayerParameters
    {
        public int MinChangePopulation { get; set; } = 0;
        public int MaxChangePopulation { get; set; } = 10000;
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }
}