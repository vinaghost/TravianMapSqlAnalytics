namespace Core.Features.Shared.Parameters
{
    public interface IPopulationHistoryFilterParameters : IHistoryParameters
    {
        int MinChangePopulation { get; }
        int MaxChangePopulation { get; }
    }
}