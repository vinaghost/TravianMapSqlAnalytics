
using WebAPI.Models.Output;

namespace WebAPI.Repositories
{
    public interface IServerRepository
    {
        Task<List<ServerRecord>> GetServerRecords(CancellationToken cancellationToken);
        Task<bool> Validate(string serverUrl, CancellationToken cancellationToken);
    }
}