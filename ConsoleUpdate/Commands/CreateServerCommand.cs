using Core;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace ConsoleUpdate.Commands
{
    public record CreateServerCommand(string ServerUrl) : ServerCommand(ServerUrl), IRequest;

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