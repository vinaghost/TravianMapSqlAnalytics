using WebAPI.Models.Output;
using WebAPI.Models.Parameters;

namespace WebAPI.Repositories
{
    public interface IPlayerRepository
    {
        IQueryable<Core.Models.PlayerEntity> GetQueryable(IPlayerFilterParameter parameters);
        Task<Dictionary<int, PlayerRecord>> GetRecords(List<int> PlayersId, CancellationToken cancellationToken);
    }
}