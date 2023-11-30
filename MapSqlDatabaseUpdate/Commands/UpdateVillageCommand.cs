﻿using MainCore;
using MapSqlDatabaseUpdate.Extensions;
using MapSqlDatabaseUpdate.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MapSqlDatabaseUpdate.Commands
{
    public class UpdateVillageCommand : VillageCommand, IRequest<int>
    {
        public UpdateVillageCommand(string serverUrl, List<VillageRaw> villageRaws) : base(serverUrl, villageRaws)
        {
        }
    }

    public class UpdateVillageCommandHandler : IRequestHandler<UpdateVillageCommand, int>
    {
        private readonly IConfiguration _configuration;

        public UpdateVillageCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<int> Handle(UpdateVillageCommand request, CancellationToken cancellationToken)
        {
            using var context = new ServerDbContext(_configuration, request.ServerUrl);
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