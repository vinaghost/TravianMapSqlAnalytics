using Core;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace ConsoleUpdate.Commands
{
    public record DeleteServerCommand(string ServerUrl) : ServerCommand(ServerUrl), IRequest;

    public class DeleteServerCommandHandler(IConfiguration configuration) : IRequestHandler<DeleteServerCommand>
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task Handle(DeleteServerCommand request, CancellationToken cancellationToken)
        {
            using var context = new ServerDbContext(_configuration["connectionStringWithoutDatabase"], request.ServerUrl);
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }
}