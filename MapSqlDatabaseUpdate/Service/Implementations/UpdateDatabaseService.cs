using MainCore;
using MainCore.DatabaseModels;
using MapSqlDatabaseUpdate.Extensions;
using MapSqlDatabaseUpdate.Models;
using MapSqlDatabaseUpdate.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MapSqlDatabaseUpdate.Service.Implementations
{
    public class UpdateDatabaseService : IUpdateDatabaseService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public UpdateDatabaseService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Execute(List<VillageRaw> villageRaws)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.Database.EnsureCreatedAsync();

            var alliances = GetAlliances(villageRaws);
            await context.BulkSynchronizeAsync(alliances, options => options.SynchronizeKeepidentity = true);

            var players = GetPlayers(villageRaws);
            await context.BulkSynchronizeAsync(players, options => options.SynchronizeKeepidentity = true);

            var villages = GetVillages(villageRaws);
            await context.BulkSynchronizeAsync(villages, options => options.SynchronizeKeepidentity = true);

            var date = DateTime.Today;
            if (!context.VillagesPopulations.Any(x => x.Date == date))
            {
                var villagePopulations = GetVillagePopulations(villages, date);
                await context.BulkInsertAsync(villagePopulations);
            }

            if (!context.PlayersAlliances.Any(x => x.Date == date))
            {
                var playerAlliances = GetPlayerAlliances(players, date);
                await context.BulkInsertAsync(playerAlliances);
            }
        }

        private static List<Alliance> GetAlliances(List<VillageRaw> villageRaws)
        {
            var alliances = villageRaws.DistinctBy(x => x.AllianceId).Select(x => x.GetAlliace()).ToList();
            return alliances;
        }

        private static List<Player> GetPlayers(List<VillageRaw> villageRaws)
        {
            var players = villageRaws.DistinctBy(x => x.PlayerId).Select(x => x.GetPlayer()).ToList();
            return players;
        }

        private static List<Village> GetVillages(List<VillageRaw> villageRaws)
        {
            var villages = villageRaws.Select(x => x.GetVillage()).ToList();
            return villages;
        }

        private static List<VillagePopulation> GetVillagePopulations(List<Village> village, DateTime date)
        {
            var villagePopulations = village.Select(x => x.GetVillagePopulation(date)).ToList();
            return villagePopulations;
        }

        private static List<PlayerAlliance> GetPlayerAlliances(List<Player> player, DateTime date)
        {
            var playerAlliance = player.Select(x => x.GetPlayerAlliance(date)).ToList();
            return playerAlliance;
        }
    }
}