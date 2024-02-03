using ConsoleUpdate.Extensions;
using ConsoleUpdate.Models;
using Core;
using Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConsoleUpdate.Commands
{
    public record UpdatePlayerCommand(ServerDbContext Context, List<VillageRaw> VillageRaws) : IRequest;

    public class UpdatePlayerCommandHandler : IRequestHandler<UpdatePlayerCommand>
    {
        public async Task Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
        {
            var context = request.Context;
            var players = request.VillageRaws
               .GroupBy(x => x.PlayerId)
               .Select(x => new Player
               {
                   Id = x.Key,
                   Name = x.First().PlayerName,
                   AllianceId = x.First().AllianceId,
                   Population = x.Sum(x => x.Population),
                   VillageCount = x.Count(),
               });

            await context.BulkSynchronizeAsync(players, options => options.SynchronizeKeepidentity = true, cancellationToken: cancellationToken);

            var today = DateTime.Today;

            if (!await context.PlayerAllianceHistory.AnyAsync(x => x.Date == today, cancellationToken: cancellationToken))
            {
                var playerAlliances = players
                    .Select(x => x.GetPlayerAlliance(today));

                await context.BulkInsertAsync(playerAlliances, cancellationToken: cancellationToken);
            }

            if (!await context.PlayerPopulationHistory.AnyAsync(x => x.Date == today, cancellationToken: cancellationToken))
            {
                var yesterday = today.AddDays(-1);
                var populationHistory = await context.PlayerPopulationHistory
                    .Where(x => x.Date == yesterday)
                    .Select(x => new
                    {
                        x.PlayerId,
                        x.Population
                    })
                    .OrderBy(x => x.PlayerId)
                    .ToListAsync(cancellationToken: cancellationToken);

                var populations = players
                    .Select(x => x.GetPlayerPopulation(DateTime.Today))
                    .ToList();

                foreach (var population in populations)
                {
                    var history = populationHistory.FirstOrDefault(x => x.PlayerId == population.PlayerId);
                    if (history is null) { continue; }
                    population.Change = population.Population - history.Population;
                }

                await context.BulkInsertAsync(populations, cancellationToken: cancellationToken);
            }
        }
    }
}