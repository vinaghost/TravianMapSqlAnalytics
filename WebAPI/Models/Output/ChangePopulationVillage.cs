namespace WebAPI.Models.Output
{
    public record ChangePopulationVillage(int VillageId, string VillageName, int X, int Y, bool IsCapital, int ChangePopulation, IEnumerable<Population> Populations);
    public record Population(int Amount, DateTime Date);
}