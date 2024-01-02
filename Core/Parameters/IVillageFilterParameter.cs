namespace Core.Parameters
{
    public interface IVillageFilterParameter : IPopulationFilterParameter, IDistanceFilterParameter
    {
        List<int> Alliances { get; }
        List<int> Players { get; }
        List<int> Villages { get; }
    }
}