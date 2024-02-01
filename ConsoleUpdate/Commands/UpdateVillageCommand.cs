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

            var today = DateTime.Today;
            if (!await context.VillagePopulationHistory.AnyAsync(x => x.Date == today, cancellationToken: cancellationToken))
            {
                var yesterday = today.AddDays(-1);
                var populationHistory = await context.VillagePopulationHistory
                    .Where(x => x.Date == yesterday)
                    .Select(x => new
                    {
                        x.VillageId,
                        x.Population
                    })
                    .OrderBy(x => x.VillageId)
                    .ToListAsync(cancellationToken: cancellationToken);

                var populations = villages
                    .Select(x => x.GetVillagePopulation(DateTime.Today))
                    .ToList();

                foreach (var population in populations)
                {
                    var history = populationHistory.FirstOrDefault(x => x.VillageId == population.VillageId);
                    if (history is null) { continue; }
                    population.Change = population.Population - history.Population;
                }
                await context.BulkInsertAsync(populations, cancellationToken: cancellationToken);
            }
            var count = await context.Villages.CountAsync(cancellationToken: cancellationToken);
            return count;
        }
    }
}