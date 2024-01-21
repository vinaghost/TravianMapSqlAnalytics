namespace Core.Features.Shared.Parameters
{
    public interface IPopulationFilterParameters
    {
        int MinPopulation { get; }
        int MaxPopulation { get; }
    }
}