using MapSqlDatabaseUpdate.Commands;
using MapSqlDatabaseUpdate.Models.Database;
using MapSqlDatabaseUpdate.Models.Raw;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace MapSqlDatabaseUpdate
{
    public class StartUpService : IHostedService
    {
        private readonly ILogger<StartUpService> _logger;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        private readonly IMediator _mediator;

        public StartUpService(IHostApplicationLifetime hostApplicationLifetime, ILogger<StartUpService> logger, IMediator mediator)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var servers = await _mediator.Send(new GetServerListCommand(), cancellationToken);

            var serverVaild = servers.Where(x => !(x.IsClosed || x.IsEnded || x.StartDate > DateTime.Now)).ToList();
            var serverInvaild = servers.Where(x => x.IsClosed || x.IsEnded || x.StartDate > DateTime.Now).ToList();

            var dataOutput = new ConcurrentQueue<Server>();
            var serverFailed = new ConcurrentQueue<ServerRaw>();

            await Parallel.ForEachAsync(serverVaild, async (server, token) =>
            {
                var output = await HandleUpdate(server);
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

            await _mediator.Send(new UpdateServerListCommand([.. dataOutput]), cancellationToken);

            var data = dataOutput.ToList();
            _logger.LogInformation("Server: {count}", data.Count);
            data.ForEach(x => _logger.LogInformation("{output}", x));
            _hostApplicationLifetime.StopApplication();
        }

        public async Task HandleDelete(string url)
        {
            await _mediator.Send(new DeleteServerCommand(url));
        }

        public async Task<Server> HandleUpdate(ServerRaw serverRaw)
        {
            var villages = await _mediator.Send(new GetMapSqlCommand(serverRaw.Url));
            if (villages.Count == 0) return null;
            await _mediator.Send(new CreateServerCommand(serverRaw.Url));

            int allianceCount = await _mediator.Send(new UpdateAllianceCommand(serverRaw.Url, villages));
            int playerCount = await _mediator.Send(new UpdatePlayerCommand(serverRaw.Url, villages));
            int villageCount = await _mediator.Send(new UpdateVillageCommand(serverRaw.Url, villages));

            var server = new Server
            {
                Id = serverRaw.Id,
                Url = serverRaw.Url,
                Zone = serverRaw.Zone,
                StartDate = serverRaw.StartDate,
                AllianceCount = allianceCount,
                PlayerCount = playerCount,
                VillageCount = villageCount,
                OasisCount = 0,
            };

            return server;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}