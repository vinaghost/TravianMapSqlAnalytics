namespace Core.Features.Shared.Parameters
{
    public interface IPopulationFilterParameter
    {
        int MinPopulation { get; }
        int MaxPopulation { get; }
    }
}