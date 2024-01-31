namespace Core.Features.Shared.Parameters
{
    public interface IPlayerPopulationFilterParameters
    {
        int MinPlayerPopulation { get; }
        int MaxPlayerPopulation { get; }
    }
}