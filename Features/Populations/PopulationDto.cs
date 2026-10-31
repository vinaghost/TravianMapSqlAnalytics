namespace Features.Populations
{
    public record PopulationDto(DateTime SnapshotDate,
                                int Population,
                                int ChangePopulation);
}