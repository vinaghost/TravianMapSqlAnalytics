namespace WebAPI.Models.Output
{
    public record ChangePopulationVillage(
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
        IEnumerable<Population> Populations);
    public record Population(
        int Amount,
        DateTime Date);
}