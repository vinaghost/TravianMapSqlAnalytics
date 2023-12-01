using MapSqlDatabaseUpdate.Context;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace MapSqlDatabaseUpdate.Commands
{
    public class DeleteServerCommand : ServerCommand, IRequest
    {
        public DeleteServerCommand(string serverUrl) : base(serverUrl)
        {
        }
    }

    public class DeleteServerCommandHandler : IRequestHandler<DeleteServerCommand>
    {
        private readonly IConfiguration _configuration;

        public DeleteServerCommandHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Handle(DeleteServerCommand request, CancellationToken cancellationToken)
        {
            using var context = new ServerDbContext(_configuration, request.ServerUrl);
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }
}