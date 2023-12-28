namespace WebAPI.Models.Parameters
{
    public class ChangePopulationPlayerParameters : PlayerParameters
    {
        public int MinChangePopulation { get; set; } = 0;
        public int MaxChangePopulation { get; set; } = 10000;
        public int Days { get; set; } = 3;
    }
}