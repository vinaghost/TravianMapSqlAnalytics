using Core;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace ConsoleUpdate.Commands
{
    public record DeleteServerCommand(string ServerUrl) : ServerCommand(ServerUrl), IRequest;

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