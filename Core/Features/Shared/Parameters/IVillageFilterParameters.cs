namespace Core.Features.Shared.Parameters
{
    public interface IVillageFilterParameters : IPopulationFilterParameters
    {
        List<int> Alliances { get; }
        List<int> Players { get; }

        int Tribe { get; }

        bool IgnoreCapital { get; }
        bool IgnoreNormalVillage { get; }
    }
}