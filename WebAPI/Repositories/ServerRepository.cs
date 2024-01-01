using Core;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Repositories
{
    public class ServerRepository(ServerListDbContext dbContext) : IServerRepository
    {
        private readonly ServerListDbContext _dbContext = dbContext;

        public async Task<bool> Validate(string serverUrl, CancellationToken cancellationToken)
        {
            return await _dbContext.Servers
                .Where(x => x.Url == serverUrl)
                .AnyAsync(cancellationToken);
        }
    }
}