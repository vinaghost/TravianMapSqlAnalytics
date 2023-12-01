using Core;
using Core.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MapSqlDatabaseUpdate.Commands
{
    public class UpdateServerListCommand : IRequest
    {
        public List<Server> Servers { get; }

        public UpdateServerListCommand(List<Server> servers)
        {
            Servers = servers;
        }
    }

    public class UpdateServerListCommandHandler : IRequestHandler<UpdateServerListCommand>
    {
        private readonly IDbContextFactory<ServerListDbContext> _contextFactory;

        public UpdateServerListCommandHandler(IDbContextFactory<ServerListDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task Handle(UpdateServerListCommand request, CancellationToken cancellationToken)
        {
            using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            await context.BulkSynchronizeAsync(request.Servers, options => options.SynchronizeKeepidentity = true);
        }
    }
}