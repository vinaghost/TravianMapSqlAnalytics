namespace Core.Models
{
    public record PlayerPopulationHistory(int ChangePopulation, IList<PopulationHistoryRecord> Populations);
}