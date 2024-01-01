
namespace WebAPI.Repositories
{
    public interface IServerRepository
    {
        Task<bool> Validate(string serverUrl, CancellationToken cancellationToken);
    }
}