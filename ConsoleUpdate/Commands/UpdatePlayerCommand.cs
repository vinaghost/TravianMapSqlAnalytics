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

            if (!await context.PlayerAllianceHistory.AnyAsync(x => x.Date == DateTime.Today, cancellationToken: cancellationToken))
            {
                var playerAlliances = players
                    .Select(x => x.GetPlayerAlliance(DateTime.Today));

                await context.BulkInsertAsync(playerAlliances, cancellationToken: cancellationToken);
            }
        }
    }
}