namespace WebAPI.Models.Parameters
{
    public interface IPopulationFilterParameter
    {
        int MinPopulation { get; }
        int MaxPopulation { get; }
    }
}