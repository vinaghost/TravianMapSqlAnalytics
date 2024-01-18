namespace Core.Models
{
    public record VillagePopulationHistory(int ChangePopulation, IList<PopulationHistoryRecord> Populations);
}