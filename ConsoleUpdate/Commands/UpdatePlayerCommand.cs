using ConsoleUpdate.Extensions;
using ConsoleUpdate.Models;
using Core;
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
               .DistinctBy(x => x.PlayerId)
               .Select(x => x.GetPlayer());
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