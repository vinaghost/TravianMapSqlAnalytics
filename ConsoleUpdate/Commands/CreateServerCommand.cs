using Core;
using Core.Config;
using MediatR;
using Microsoft.Extensions.Options;

namespace ConsoleUpdate.Commands
{
    public record CreateServerCommand(string ServerUrl) : IRequest<ServerDbContext>;

    public class CreateServerCommandHandler(IOptions<ConnectionStringOption> connectionStringOption) : IRequestHandler<CreateServerCommand, ServerDbContext>
    {
        private readonly string _connectionString = connectionStringOption.Value.DataSource;

        public async Task<ServerDbContext> Handle(CreateServerCommand request, CancellationToken cancellationToken)
        {
            var context = new ServerDbContext(_connectionString, request.ServerUrl);
            await context.Database.EnsureCreatedAsync(cancellationToken);
            return context;
        }
    }
}