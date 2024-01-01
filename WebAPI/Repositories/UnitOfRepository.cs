namespace WebAPI.Repositories
{
    public record UnitOfRepository(
        IAllianceRepository AllianceRepository,
        IPlayerRepository PlayerRepository,
        IVillageRepository VillageRepository
        );
}