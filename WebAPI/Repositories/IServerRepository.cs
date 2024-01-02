
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using X.PagedList;

namespace WebAPI.Repositories
{
    public interface IServerRepository
    {
        Task<List<ServerRecord>> GetServerRecords(CancellationToken cancellationToken);
        Task<IPagedList<Server>> GetServers(IPaginationParameters parameters);
        Task<bool> Validate(string serverUrl, CancellationToken cancellationToken);
    }
}