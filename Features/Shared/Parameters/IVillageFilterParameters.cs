using Features.Shared.Enums;

namespace Features.Shared.Parameters
{
    public interface IVillageFilterParameters
    {
        int MinVillagePopulation { get; }
        int MaxVillagePopulation { get; }

        Capital Capital { get; }

        Tribe Tribe { get; }
    }
}