namespace Core.Models
{
    public record VillageContainPopulationHistory(
        int PlayerId,
        int VillageId,
        string VillageName,
        int X,
        int Y,
        bool IsCapital,
        int Tribe,
        double Distance,
        int ChangePopulation,
        IEnumerable<PopulationHistoryRecord> Populations);

    public record VillageContainPopulationHistoryDetail(
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
        IEnumerable<PopulationHistoryRecord> Populations) : VillageContainPopulationHistory(PlayerId, VillageId, VillageName, X, Y, IsCapital, Tribe, Distance, ChangePopulation, Populations);
}