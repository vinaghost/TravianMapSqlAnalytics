using MainCore.Models;
using MapSqlDatabaseUpdate.Commands;
using MapSqlDatabaseUpdate.Models;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace MapSqlDatabaseUpdate
{
    public class StartUpService : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<StartUpService> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        private readonly IMediator _mediator;

        public StartUpService(IConfiguration configuration, IHostApplicationLifetime hostApplicationLifetime, ILogger<StartUpService> logger, IMediator mediator)
        {
            _configuration = configuration;
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var servers = await _mediator.Send(new GetServerListCommand(), cancellationToken);

            var serverVaild = servers.Where(x => !(x.IsClosed || x.IsEnded || x.Start > DateTime.Now)).ToList();
            var serverInvaild = servers.Where(x => x.IsClosed || x.IsEnded || x.Start > DateTime.Now).ToList();

            var dataOutput = new ConcurrentQueue<Output>();
            var serverFailed = new ConcurrentQueue<Server>();

            await Parallel.ForEachAsync(serverVaild, async (server, token) =>
            {
                var output = await HandleUpdate(server.Url);
                if (output is null)
                {
                    serverFailed.Enqueue(server);
                }
                else
                {
                    dataOutput.Enqueue(output);
                }
            });

            serverInvaild.AddRange(serverFailed);
            foreach (var server in serverFailed)
            {
                serverVaild.Remove(server);
            }
            await Parallel.ForEachAsync(serverInvaild, async (server, token) => await HandleDelete(server.Url));

            await _mediator.Send(new UpdateServerListCommand(serverVaild), cancellationToken);

            var data = dataOutput.ToList();
            _logger.LogInformation("Server: {count}", data.Count);
            data.ForEach(x => _logger.LogInformation("{output}", x));
            _hostApplicationLifetime.StopApplication();
        }

        public async Task HandleDelete(string url)
        {
            await _mediator.Send(new DeleteServerCommand(url));
        }

        public async Task<Output> HandleUpdate(string url)
        {
            var villages = await _mediator.Send(new GetMapSqlCommand(url));
            if (villages.Count == 0) return null;
            await _mediator.Send(new CreateServerCommand(url));

            int allianceCount = await _mediator.Send(new UpdateAllianceCommand(url, villages));
            int playerCount = await _mediator.Send(new UpdatePlayerCommand(url, villages));
            int villageCount = await _mediator.Send(new UpdateVillageCommand(url, villages));

            var output = new Output
            {
                ServerUrl = url,
                AllianceCount = allianceCount,
                PlayerCount = playerCount,
                VillageCount = villageCount,
            };

            return output;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}