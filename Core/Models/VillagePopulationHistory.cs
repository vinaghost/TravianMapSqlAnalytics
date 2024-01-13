namespace Core.Models
{
    public record VillagePopulationHistory(double Distance, int ChangePopulation, IList<PopulationHistoryRecord> Populations);
}