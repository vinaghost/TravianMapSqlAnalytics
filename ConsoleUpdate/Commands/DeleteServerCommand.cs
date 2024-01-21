using Core;
using Core.Config;
using MediatR;
using Microsoft.Extensions.Options;

namespace ConsoleUpdate.Commands
{
    public record DeleteServerCommand(string ServerUrl) : IRequest;

    public class DeleteServerCommandHandler(IOptions<ConnectionStringOption> connectionStringOption) : IRequestHandler<DeleteServerCommand>
    {
        private readonly string _connectionString = connectionStringOption.Value.DataSource;

        public async Task Handle(DeleteServerCommand request, CancellationToken cancellationToken)
        {
            using var context = new ServerDbContext(_connectionString, request.ServerUrl);
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }
}