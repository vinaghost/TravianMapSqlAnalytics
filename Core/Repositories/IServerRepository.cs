using Core.Models;
using Core.Parameters;
using X.PagedList;

namespace Core.Repositories
{
    public interface IServerRepository
    {
        Task<IEnumerable<ServerRecord>> GetServerRecords(CancellationToken cancellationToken);

        Task<IPagedList<Server>> GetServers(IPaginationParameters parameters);
    }
}