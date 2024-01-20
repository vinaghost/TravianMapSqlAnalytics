using Core.Features.Shared.Models;

namespace Core.Features.GetVillageContainsPopulationHistory
{
    public record VillageContainsPopulationHistoryDto(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int VillageId,
        string VillageName,
        int X,
        int Y,
        bool IsCapital,
        int Tribe,
        int ChangePopulation,
        IList<PopulationHistoryRecord> Populations);
}