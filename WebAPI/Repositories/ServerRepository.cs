using Core;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using X.PagedList;

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

        public async Task<List<ServerRecord>> GetServerRecords(CancellationToken cancellationToken)
        {
            return await _dbContext.Servers
                .Select(x => new ServerRecord(x.Url))
                .ToListAsync(cancellationToken);
        }

        public async Task<IPagedList<Server>> GetServers(IPaginationParameters parameters)
        {
            return await _dbContext.Servers
                .OrderByDescending(x => x.PlayerCount)
                .Select(x => new Server(x.Url, x.Zone, x.StartDate, x.AllianceCount, x.PlayerCount, x.VillageCount))
                .ToPagedListAsync(parameters.PageNumber, parameters.PageSize);
        }
    }
}