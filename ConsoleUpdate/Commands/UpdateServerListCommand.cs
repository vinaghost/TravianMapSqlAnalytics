using Core;
using Core.Entities;
using MediatR;

namespace ConsoleUpdate.Commands
{
    public record UpdateServerListCommand(List<Server> Servers) : IRequest;

    public class UpdateServerListCommandHandler(ServerListDbContext dbContext) : IRequestHandler<UpdateServerListCommand>
    {
        private readonly ServerListDbContext _dbContext = dbContext;

        public async Task Handle(UpdateServerListCommand request, CancellationToken cancellationToken)
        {
            await _dbContext.Database.EnsureCreatedAsync(cancellationToken);
            await _dbContext.BulkSynchronizeAsync(request.Servers, options => options.SynchronizeKeepidentity = true);
        }
    }
}