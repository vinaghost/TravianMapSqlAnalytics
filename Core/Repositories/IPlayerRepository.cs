using Core.Entities;
using Core.Models;
using Core.Parameters;

namespace Core.Repositories
{
    public interface IPlayerRepository
    {
        IQueryable<Entities.Player> GetQueryable(IPlayerFilterParameter parameters);
        Task<Dictionary<int, PlayerRecord>> GetRecords(List<int> PlayersId, CancellationToken cancellationToken);
    }
}