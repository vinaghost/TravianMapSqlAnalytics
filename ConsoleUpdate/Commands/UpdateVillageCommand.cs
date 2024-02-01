using ConsoleUpdate.Extensions;
using ConsoleUpdate.Models;
using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ConsoleUpdate.Commands
{
    public record UpdateVillageCommand(ServerDbContext Context, List<VillageRaw> VillageRaws) : IRequest;

    public class UpdateVillageCommandHandler : IRequestHandler<UpdateVillageCommand>
    {
        public async Task Handle(UpdateVillageCommand request, CancellationToken cancellationToken)
        {
            var context = request.Context;
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
        }
    }
}