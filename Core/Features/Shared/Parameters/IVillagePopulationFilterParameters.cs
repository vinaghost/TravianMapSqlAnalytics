namespace Core.Features.Shared.Parameters
{
    public interface IVillagePopulationFilterParameters
    {
        int MinVillagePopulation { get; }
        int MaxVillagePopulation { get; }
    }
}