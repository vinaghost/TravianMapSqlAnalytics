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

            foreach (var alliance in alliances)
            {
                var query = context.Alliances
                    .Where(x => x.AllianceId == alliance.AllianceId);
                if (await query.AnyAsync())
                {
                    await query.ExecuteUpdateAsync(x => x
                        .SetProperty(x => x.Name, x => alliance.Name)
                    );
                }
                else
                {
                    await context.AddAsync(alliance);
                }
            }
            await context.SaveChangesAsync();

            var count = await context.Alliances
                .AsNoTracking()
                .CountAsync();
            _logger.LogInformation("Database has {count} alliances", count);
        }

        private async Task ExecutePlayers(ServerDbContext context, List<VillageRaw> villageRaws)
        {
            var players = villageRaws
                .DistinctBy(x => x.PlayerId)
                .Select(x => x.GetPlayer());

            var playerIds = context.Players
                .Select(x => x.PlayerId)
                .AsEnumerable();
            var oldPlayers = new List<int>();
            foreach (var playerId in playerIds)
            {
                var player = players.FirstOrDefault(x => x.PlayerId == playerId);
                if (player is not null)
                {
                    await context.Players
                        .ExecuteUpdateAsync(x => x
                            .SetProperty(x => x.Name, x => player.Name)
                            .SetProperty(x => x.AllianceId, x => player.AllianceId)
                    );
                }
                else
                {
                    await context.Villages
                        .Where(x => x.PlayerId == playerId)
                        .ExecuteDeleteAsync();
                }

                oldPlayers.Add(playerId);
            }

            var newPlayers = players
                .Where(x => !oldPlayers.Contains(x.PlayerId));

            foreach (var player in newPlayers)
            {
                await context.AddAsync(player);
            }
            await context.SaveChangesAsync();

            var count = await context.Players
                .AsNoTracking()
                .CountAsync();
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

            var villageIds = context.Villages
                .Select(x => x.VillageId)
                .AsEnumerable();

            var oldVillages = new List<int>();
            foreach (var villageId in villageIds)
            {
                var village = villages.FirstOrDefault(x => x.VillageId == villageId);
                if (village is not null)
                {
                    await context.Villages
                        .ExecuteUpdateAsync(x => x
                            .SetProperty(x => x.PlayerId, x => village.PlayerId)
                            .SetProperty(x => x.Name, x => village.Name)
                            .SetProperty(x => x.X, x => village.X)
                            .SetProperty(x => x.Y, x => village.Y)
                            .SetProperty(x => x.Tribe, x => village.Tribe)
                            .SetProperty(x => x.Population, x => village.Population)
                            .SetProperty(x => x.IsCapital, x => village.IsCapital)
                            .SetProperty(x => x.IsCity, x => village.IsCity)
                            .SetProperty(x => x.IsHarbor, x => village.IsHarbor)
                            .SetProperty(x => x.VictoryPoints, x => village.VictoryPoints)
                    );
                }
                else
                {
                    await context.Villages
                        .Where(x => x.VillageId == villageId)
                        .ExecuteDeleteAsync();
                }

                oldVillages.Add(villageId);
            }

            var newVillages = villages
                .Where(x => !oldVillages.Contains(x.VillageId));

            foreach (var village in newVillages)
            {
                await context.AddAsync(village);
            }
            await context.SaveChangesAsync();

            var count = await context.Villages
                .AsNoTracking()
                .CountAsync();
            _logger.LogInformation("Database has {count} players", count);

            if (!context.VillagesPopulations.Any(x => x.Date == DateTime.Today))
            {
                var villagePopulations = villages
                    .Select(x => x.GetVillagePopulation(DateTime.Today));
                await context.BulkInsertAsync(villagePopulations);
            }
        }
    }
}