﻿using ConsoleTables;
using ConsoleUpdate.Commands;
using ConsoleUpdate.Models;
using Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;

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
            Console.WriteLine("Found {0} servers.", servers.Count);

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
            Console.WriteLine("{0} servers are alive. Updating...", servers.Count);

            var serversInfo = new ConcurrentQueue<Server>();

            await Parallel.ForEachAsync(servers, async (server, token) =>
            {
                var sw = new Stopwatch();
                sw.Start();
                var serverInfo = await HandleUpdate(server, cancellationToken);
                sw.Stop();
                if (serverInfo is null) return;
                serversInfo.Enqueue(serverInfo);
                Console.WriteLine("Updated {0} in {1}s", server.Url, sw.ElapsedMilliseconds / 1000);
            });

            await _mediator.Send(new UpdateServerListCommand([.. serversInfo]), cancellationToken);

            var data = serversInfo.OrderByDescending(x => x.PlayerCount).ToList();
            ConsoleTable
                .From(data)
                .Configure(o => o.NumberAlignment = Alignment.Right)
                .Write(Format.Alternative);
            _hostApplicationLifetime.StopApplication();
        }

        private async Task HandleDelete(string url, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteServerCommand(url), cancellationToken);
        }

        private async Task<Server?> HandleUpdate(ServerRaw serverRaw, CancellationToken cancellationToken)
        {
            var villages = await _mediator.Send(new GetMapSqlCommand(serverRaw.Url), cancellationToken);
            if (villages.Count == 0) return null;

            using var context = await _mediator.Send(new CreateServerCommand(serverRaw.Url), cancellationToken);

            var transaction = context.Database.BeginTransaction();
            try
            {
                await _mediator.Send(new UpdateAllianceCommand(context, villages), cancellationToken);
                await _mediator.Send(new UpdatePlayerCommand(context, villages), cancellationToken);
                await _mediator.Send(new UpdateVillageCommand(context, villages), cancellationToken);
                transaction.Commit();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{message}", e.Message);
                transaction.Rollback();
            }
            var allianceCount = await context.Alliances.CountAsync(cancellationToken: cancellationToken);
            var playerCount = await context.Players.CountAsync(cancellationToken: cancellationToken);
            var villageCount = await context.Villages.CountAsync(cancellationToken: cancellationToken);
            var oasisCount = 0;

            var server = new Server
            {
                Id = serverRaw.Id,
                Url = serverRaw.Url,
                LastUpdate = DateTime.Now,
                AllianceCount = allianceCount,
                PlayerCount = playerCount,
                VillageCount = villageCount,
                OasisCount = oasisCount,
            };

            return server;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}