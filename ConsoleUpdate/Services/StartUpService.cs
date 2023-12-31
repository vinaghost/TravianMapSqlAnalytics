﻿using ConsoleTables;
using ConsoleUpdate.Commands;
using ConsoleUpdate.Models;
using Core.Entities;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace ConsoleUpdate.Services
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
            _logger.LogInformation("Found {count} servers", servers.Count);

            var serverVaild = servers.Where(x => !(x.IsClosed || x.IsEnded || x.StartDate > DateTime.Now)).ToList();
            var serverInvaild = servers.Where(x => x.IsClosed || x.IsEnded || x.StartDate > DateTime.Now).ToList();

            var serverFailed = new ConcurrentQueue<ServerRaw>();

            await Parallel.ForEachAsync(serverVaild, async (server, token) =>
            {
                var isValid = await _mediator.Send(new ValidateServerCommand(server.Url), token);
                if (!isValid)
                {
                    serverFailed.Enqueue(server);
                }
            });

            serverInvaild.AddRange(serverFailed);
            _logger.LogInformation("{count} servers are dead. Deleting their database to free space ...", serverInvaild.Count);
            await Parallel.ForEachAsync(serverInvaild, async (server, token) => await HandleDelete(server.Url));

            foreach (var server in serverFailed)
            {
                serverVaild.Remove(server);
            }
            _logger.LogInformation("{count} servers are alive. Updating...", serverVaild.Count);

            var serversInfo = new ConcurrentQueue<Server>();
            await Parallel.ForEachAsync(serverVaild, async (server, token) =>
            {
                var serverInfo = await HandleUpdate(server);
                if (serverInfo is null) return;
                serversInfo.Enqueue(serverInfo);
            });

            await _mediator.Send(new UpdateServerListCommand([.. serversInfo]), cancellationToken);

            var data = serversInfo.OrderByDescending(x => x.PlayerCount).ToList();
            ConsoleTable
                .From(data)
                .Configure(o => o.NumberAlignment = Alignment.Right)
                .Write(Format.Alternative);
            _hostApplicationLifetime.StopApplication();
        }

        public async Task HandleDelete(string url)
        {
            await _mediator.Send(new DeleteServerCommand(url));
        }

        public async Task<Server?> HandleUpdate(ServerRaw serverRaw)
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