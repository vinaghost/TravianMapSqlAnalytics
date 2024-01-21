using ConsoleUpdate.Extensions;
using ConsoleUpdate.Models;
using Core;
using Core.Config;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ConsoleUpdate.Commands
{
    public record UpdateVillageCommand(string ServerUrl, List<VillageRaw> VillageRaws) : IRequest<int>;

    public class UpdateVillageCommandHandler(IOptions<ConnectionStringOption> connectionStringOption) : IRequestHandler<UpdateVillageCommand, int>
    {
        private readonly string _connectionString = connectionStringOption.Value.DataSource;

        public async Task<int> Handle(UpdateVillageCommand request, CancellationToken cancellationToken)
        {
            using var context = new ServerDbContext(_connectionString, request.ServerUrl);
            var villages = request.VillageRaws
                .Select(x => x.GetVillage());
            await context.BulkSynchronizeAsync(villages, options => options.SynchronizeKeepidentity = true, cancellationToken: cancellationToken);

            if (!await context.VillagesPopulations.AnyAsync(x => x.Date == DateTime.Today, cancellationToken: cancellationToken))
            {
                var villagePopulations = villages
                    .Select(x => x.GetVillagePopulation(DateTime.Today));
                await context.BulkInsertAsync(villagePopulations, cancellationToken: cancellationToken);
            }
            var count = await context.Villages.CountAsync(cancellationToken: cancellationToken);
            return count;
        }
    }
}