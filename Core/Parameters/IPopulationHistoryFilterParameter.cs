namespace Core.Parameters
{
    public interface IPopulationHistoryFilterParameter : IHistoryParameters
    {
        int MinChangePopulation { get; }
        int MaxChangePopulation { get; }
    }
}