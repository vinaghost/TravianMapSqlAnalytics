using MainCore;
using MainCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MapSqlDatabaseUpdate.CQRS.Commands
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

            await context.BulkMergeAsync(request.Servers, options => options.MergeKeepIdentity = true);
        }
    }
}