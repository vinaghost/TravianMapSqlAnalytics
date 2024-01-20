namespace Core.Features.Shared.Parameters
{
    public interface IVillageFilterParameter : IPopulationFilterParameter
    {
        List<int> Alliances { get; }
        List<int> Players { get; }

        int Tribe { get; }

        bool IgnoreCapital { get; }
        bool IgnoreNormalVillage { get; }
    }
}