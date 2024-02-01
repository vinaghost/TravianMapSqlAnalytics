using ConsoleUpdate.Extensions;
using ConsoleUpdate.Models;
using Core;
using Core.Config;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConsoleUpdate.Commands
{
    public record UpdatePlayerCommand(string ServerUrl, List<VillageRaw> VillageRaws) : IRequest<int>;

    public class UpdatePlayerCommandHandler(IOptions<ConnectionStringOption> connectionStringOption) : IRequestHandler<UpdatePlayerCommand, int>
    {
        private readonly string _connectionString = connectionStringOption.Value.DataSource;

        public async Task<int> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
        {
            using var context = new ServerDbContext(_connectionString, request.ServerUrl);
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
            var count = await context.Players.CountAsync(cancellationToken: cancellationToken);
            return count;
        }
    }
}