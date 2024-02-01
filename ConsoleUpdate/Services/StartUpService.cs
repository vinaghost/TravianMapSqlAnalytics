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

            var serverFailed = new ConcurrentQueue<ServerRaw>();

            await Parallel.ForEachAsync(servers, async (server, token) =>
            {
                var isValid = await _mediator.Send(new ValidateServerCommand(server.Url), token);
                if (!isValid)
                {
                    serverFailed.Enqueue(server);
                }
            });

            foreach (var server in serverFailed)
            {
                servers.Remove(server);
            }
            _logger.LogInformation("{count} servers are alive. Updating...", servers.Count);

            var serversInfo = new List<Server>();
            foreach (var server in servers)
            {
                var serverInfo = await HandleUpdate(server);
                if (serverInfo is null) return;
                serversInfo.Add(serverInfo);
            }

            await _mediator.Send(new UpdateServerListCommand([.. serversInfo]), cancellationToken);

            var data = serversInfo.OrderByDescending(x => x.PlayerCount).ToList();
            ConsoleTable
                .From(data)
                .Configure(o => o.NumberAlignment = Alignment.Right)
                .Write(Format.Alternative);
            _hostApplicationLifetime.StopApplication();
        }

        private async Task HandleDelete(string url)
        {
            await _mediator.Send(new DeleteServerCommand(url));
        }

        private async Task<Server?> HandleUpdate(ServerRaw serverRaw)
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
                LastUpdate = DateTime.Now,
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