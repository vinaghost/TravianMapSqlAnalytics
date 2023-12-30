namespace WebAPI.Models.Parameters
{
    public interface IChangePopulationFilterParameter : IChangeParameters
    {
        int MinChangePopulation { get; }
        int MaxChangePopulation { get; }
    }
}