namespace WebAPI.Models.Parameters
{
    public interface IPopulationHistoryFilterParameter : IHistoryParameters
    {
        int MinChangePopulation { get; }
        int MaxChangePopulation { get; }
    }
}