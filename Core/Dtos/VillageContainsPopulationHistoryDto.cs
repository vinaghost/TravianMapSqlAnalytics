using Core.Models;

namespace Core.Dtos
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
        double Distance,
        int ChangePopulation,
        IList<PopulationHistoryRecord> Populations);
}