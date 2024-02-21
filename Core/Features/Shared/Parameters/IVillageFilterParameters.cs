using Core.Features.Shared.Enums;

namespace Core.Features.Shared.Parameters
{
    public interface IVillageFilterParameters
    {
        int MinVillagePopulation { get; }
        int MaxVillagePopulation { get; }

        Capital Capital { get; }

        Tribe Tribe { get; }
    }
}