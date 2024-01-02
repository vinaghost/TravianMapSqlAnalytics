namespace Core.Parameters
{
    public interface IPopulationFilterParameter
    {
        int MinPopulation { get; }
        int MaxPopulation { get; }
    }
}