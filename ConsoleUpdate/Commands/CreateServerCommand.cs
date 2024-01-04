using Core;
using Core.Config;
using MediatR;
using Microsoft.Extensions.Options;

namespace ConsoleUpdate.Commands
{
    public record CreateServerCommand(string ServerUrl) : IRequest;

    public class CreateServerCommandHandler(IOptions<ConnectionStringOption> connectionStringOption) : IRequestHandler<CreateServerCommand>
    {
        private readonly string _connectionString = connectionStringOption.Value.Value;

        public async Task Handle(CreateServerCommand request, CancellationToken cancellationToken)
        {
            using var context = new ServerDbContext(_connectionString, request.ServerUrl);
            await context.Database.EnsureCreatedAsync(cancellationToken);
        }
    }
}