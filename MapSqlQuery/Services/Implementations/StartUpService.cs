using MapSqlQuery.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace MapSqlQuery.Services.Implementations
{
    public class StartUpService : IHostedService
    {
        private readonly ILogger<StartUpService> _logger;

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IDataUpdate _dataUpdater;
        private readonly IDataProvide _dataProvider;

        public StartUpService(IDbContextFactory<AppDbContext> contextFactory, IDataUpdate dataUpdate, IDataProvide dataProvider, ILogger<StartUpService> logger)
        {
            _contextFactory = contextFactory;
            _dataUpdater = dataUpdate;
            _dataProvider = dataProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var stopWatch = new Stopwatch();

            stopWatch.Start();
            _logger.LogInformation("Creating database");
            EnsureCreated();
            stopWatch.Stop();
            _logger.LogInformation("Database created in {time} ms", stopWatch.ElapsedMilliseconds);

            stopWatch.Restart();
            _logger.LogInformation("Getting newest date");
            _dataProvider.NewestDate = await _dataUpdater.GetNewestDate();
            stopWatch.Stop();
            _logger.LogInformation("Newest date is {date:dd/MM/yyyy} in {time} ms", _dataProvider.NewestDate, stopWatch.ElapsedMilliseconds);

            stopWatch.Restart();
            _logger.LogInformation("Updating alliances");
            await _dataUpdater.UpdateAlliances();
            stopWatch.Stop();
            _logger.LogInformation("Alliances updated in {time} ms", stopWatch.ElapsedMilliseconds);

            stopWatch.Restart();
            _logger.LogInformation("Updating players");
            await _dataUpdater.UpdatePlayer();
            stopWatch.Stop();
            _logger.LogInformation("Players updated in {time} ms", stopWatch.ElapsedMilliseconds);

            stopWatch.Restart();
            _logger.LogInformation("Updating villages");
            await _dataUpdater.UpdateVillage();
            stopWatch.Stop();
            _logger.LogInformation("Villages updated in {time} ms", stopWatch.ElapsedMilliseconds);

            stopWatch.Restart();
            Parallel.For(0, 7, async (i) =>
            {
                var date = _dataProvider.NewestDate.AddDays(-i);
                _logger.LogInformation("Updating villages population [{date:dd/MM/yyyy}]", date);
                await _dataUpdater.UpdatePopulation(date);
            });
            stopWatch.Stop();
            _logger.LogInformation("Villages population updated in {time} ms", stopWatch.ElapsedMilliseconds);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void EnsureCreated()
        {
            using var context = _contextFactory.CreateDbContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}