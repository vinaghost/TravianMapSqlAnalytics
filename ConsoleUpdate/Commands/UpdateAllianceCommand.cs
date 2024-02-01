using ConsoleUpdate.Extensions;
using ConsoleUpdate.Models;
using Core;
using MediatR;

namespace ConsoleUpdate.Commands
{
    public record UpdateAllianceCommand(ServerDbContext Context, List<VillageRaw> VillageRaws) : IRequest;

    public class UpdateAllianceCommandHandler : IRequestHandler<UpdateAllianceCommand>
    {
        public async Task Handle(UpdateAllianceCommand request, CancellationToken cancellationToken)
        {
            var context = request.Context;
            var alliances = request.VillageRaws
                .DistinctBy(x => x.AllianceId)
                .Select(x => x.GetAlliace());
            await context.BulkMergeAsync(alliances);
        }
    }
}