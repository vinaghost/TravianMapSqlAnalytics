namespace Core.Features.Shared.Parameters
{
    public interface IPopulationHistoryFilterParameter : IHistoryParameters
    {
        int MinChangePopulation { get; }
        int MaxChangePopulation { get; }
    }
}