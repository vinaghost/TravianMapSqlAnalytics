using ConsoleUpdate.Extensions;
using ConsoleUpdate.Models;
using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ConsoleUpdate.Commands
{
    public record UpdatePlayerCommand(string ServerUrl, List<VillageRaw> VillageRaws) : VillageCommand(ServerUrl, VillageRaws), IRequest<int>;

    public class UpdatePlayerCommandHandler : IRequestHandler<UpdatePlayerCommand, int>
    {
        private readonly IConfiguration _configuration;

        public UpdatePlayerCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
        {
            using var context = new ServerDbContext(_configuration, request.ServerUrl);
            var players = request.VillageRaws
               .DistinctBy(x => x.PlayerId)
               .Select(x => x.GetPlayer());
            await context.BulkSynchronizeAsync(players, options => options.SynchronizeKeepidentity = true, cancellationToken: cancellationToken);

            if (!await context.PlayersAlliances.AnyAsync(x => x.Date == DateTime.Today, cancellationToken: cancellationToken))
            {
                var playerAlliances = players
                    .Select(x => x.GetPlayerAlliance(DateTime.Today));
                await context.BulkInsertAsync(playerAlliances, cancellationToken: cancellationToken);
            }
            var count = await context.Players.CountAsync(cancellationToken: cancellationToken);
            return count;
        }
    }
}