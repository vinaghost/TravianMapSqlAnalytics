using MainCore;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace MapSqlDatabaseUpdate.Commands
{
    public class CreateServerCommand : ServerCommand, IRequest
    {
        public CreateServerCommand(string serverUrl) : base(serverUrl)
        {
        }
    }

    public class CreateServerCommandHandler : IRequestHandler<CreateServerCommand>
    {
        private readonly IConfiguration _configuration;

        public CreateServerCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Handle(CreateServerCommand request, CancellationToken cancellationToken)
        {
            using var context = new ServerDbContext(_configuration, request.ServerUrl);
            await context.Database.EnsureCreatedAsync(cancellationToken);
        }
    }
}