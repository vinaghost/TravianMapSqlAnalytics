using MainCore;
using MapSqlDatabaseUpdate.Extensions;
using MapSqlDatabaseUpdate.Models;
using MapSqlDatabaseUpdate.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MapSqlDatabaseUpdate.Service.Implementations
{
    public class UpdateDatabaseService : IUpdateDatabaseService
    {
        private readonly IDbContextFactory<ServerDbContext> _contextFactory;
        private readonly ILogger<UpdateDatabaseService> _logger;

        public UpdateDatabaseService(IDbContextFactory<ServerDbContext> contextFactory, ILogger<UpdateDatabaseService> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task Execute(List<VillageRaw> villageRaws)
        {
            using var context = _contextFactory.CreateDbContext();
            await context.Database.EnsureCreatedAsync();
            await ExecuteAlliances(context, villageRaws);
            await ExecutePlayers(context, villageRaws);
            await ExecuteVillages(context, villageRaws);
        }

        private async Task ExecuteAlliances(ServerDbContext context, List<VillageRaw> villageRaws)
        {
            var alliances = villageRaws
                .DistinctBy(x => x.AllianceId)
                .Select(x => x.GetAlliace());
            await context.BulkMergeAsync(alliances, options => options.MergeKeepIdentity = true);
            var count = await context.Alliances.CountAsync();
            _logger.LogInformation("Database has {count} alliances", count);
        }

        private async Task ExecutePlayers(ServerDbContext context, List<VillageRaw> villageRaws)
        {
            var players = villageRaws
                .DistinctBy(x => x.PlayerId)
                .Select(x => x.GetPlayer());
            await context.BulkSynchronizeAsync(players, options => options.MergeKeepIdentity = true);
            var count = await context.Players.CountAsync();
            _logger.LogInformation("Database has {count} players", count);

            if (!context.PlayersAlliances.Any(x => x.Date == DateTime.Today))
            {
                var playerAlliances = players
                    .Select(x => x.GetPlayerAlliance(DateTime.Today));
                await context.BulkInsertAsync(playerAlliances);
            }
        }

        private async Task ExecuteVillages(ServerDbContext context, List<VillageRaw> villageRaws)
        {
            var villages = villageRaws
                .Select(x => x.GetVillage());
            await context.BulkSynchronizeAsync(villages, options => options.SynchronizeKeepidentity = true);
            var count = await context.Players.CountAsync();
            _logger.LogInformation("Database has {count} villages", count);

            if (!context.VillagesPopulations.Any(x => x.Date == DateTime.Today))
            {
                var villagePopulations = villages
                    .Select(x => x.GetVillagePopulation(DateTime.Today));
                await context.BulkInsertAsync(villagePopulations);
            }
        }
    }
}