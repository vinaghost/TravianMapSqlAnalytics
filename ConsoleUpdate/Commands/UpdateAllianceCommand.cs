using ConsoleUpdate.Extensions;
using ConsoleUpdate.Models;
using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ConsoleUpdate.Commands
{
    public record UpdateAllianceCommand(string ServerUrl, List<VillageRaw> VillageRaws) : VillageCommand(ServerUrl, VillageRaws), IRequest<int>;

    public class UpdateAllianceCommandHandler : IRequestHandler<UpdateAllianceCommand, int>
    {
        private readonly IConfiguration _configuration;

        public UpdateAllianceCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> Handle(UpdateAllianceCommand request, CancellationToken cancellationToken)
        {
            using var context = new ServerDbContext(_configuration, request.ServerUrl);
            var alliances = request.VillageRaws
                .DistinctBy(x => x.AllianceId)
                .Select(x => x.GetAlliace());
            await context.BulkMergeAsync(alliances, options => options.MergeKeepIdentity = true, cancellationToken: cancellationToken);
            var count = await context.Alliances.CountAsync(cancellationToken: cancellationToken);
            return count;
        }
    }
}