using Core;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace ConsoleUpdate.Commands
{
    public record CreateServerCommand(string ServerUrl) : ServerCommand(ServerUrl), IRequest;

    public class CreateServerCommandHandler(IConfiguration configuration) : IRequestHandler<CreateServerCommand>
    {
        private readonly IConfiguration _configuration = configuration;

        public async Task Handle(CreateServerCommand request, CancellationToken cancellationToken)
        {
            using var context = new ServerDbContext(_configuration["connectionStringWithoutDatabase"], request.ServerUrl);
            await context.Database.EnsureCreatedAsync(cancellationToken);
        }
    }
}