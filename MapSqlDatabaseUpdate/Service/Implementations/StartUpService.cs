using MapSqlDatabaseUpdate.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MapSqlDatabaseUpdate.Service.Implementations
{
    public class StartUpService : IHostedService
    {
        private readonly IGetFileService _getFileService;
        private readonly IParseService _parseService;
        private readonly IUpdateDatabaseService _updateDatabaseService;

        private readonly IConfiguration _configuration;
        private readonly ILogger<StartUpService> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public StartUpService(IGetFileService getFileService, IParseService parseService, IConfiguration configuration, IHostApplicationLifetime hostApplicationLifetime, ILogger<StartUpService> logger, IUpdateDatabaseService updateDatabaseService)
        {
            _getFileService = getFileService;
            _parseService = parseService;
            _configuration = configuration;
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
            _updateDatabaseService = updateDatabaseService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var worldUrl = _configuration["WorldUrl"];
            if (string.IsNullOrEmpty(worldUrl))
            {
                _logger.LogWarning("WorldUrl variable is empty");
                _hostApplicationLifetime.StopApplication();
                return;
            }
            var villageLines = await _getFileService.GetMapSql(worldUrl);

            if (string.IsNullOrEmpty(villageLines))
            {
                _logger.LogWarning("{world} doesn't any village in map.sql", worldUrl);
                _hostApplicationLifetime.StopApplication();
                return;
            }
            var villages = _parseService.GetVillages(villageLines);
            await _updateDatabaseService.Execute(villages);
            _hostApplicationLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}