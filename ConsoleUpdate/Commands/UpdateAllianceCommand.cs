using ConsoleUpdate.Extensions;
using ConsoleUpdate.Models;
using Core;
using Core.Config;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConsoleUpdate.Commands
{
    public record UpdateAllianceCommand(string ServerUrl, List<VillageRaw> VillageRaws) : IRequest<int>;

    public class UpdateAllianceCommandHandler(IOptions<ConnectionStringOption> connectionStringOption) : IRequestHandler<UpdateAllianceCommand, int>
    {
        private readonly string _connectionString = connectionStringOption.Value.DataSource;

        public async Task<int> Handle(UpdateAllianceCommand request, CancellationToken cancellationToken)
        {
            using var context = new ServerDbContext(_connectionString, request.ServerUrl);
            var alliances = request.VillageRaws
                .DistinctBy(x => x.AllianceId)
                .Select(x => x.GetAlliace());
            await context.BulkMergeAsync(alliances, options => options.MergeKeepIdentity = true, cancellationToken: cancellationToken);
            var count = await context.Alliances.CountAsync(cancellationToken: cancellationToken);
            return count;
        }
    }
}