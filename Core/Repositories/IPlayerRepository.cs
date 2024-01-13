using Core.Dtos;
using Core.Models;
using Core.Parameters;

namespace Core.Repositories
{
    public interface IPlayerRepository
    {
        Task<Dictionary<int, PlayerAllianceHistory>> GetPlayerAllianceHistory(IList<int> playerIds, PlayerContainsAllianceHistoryParameters parameters, CancellationToken cancellationToken);

        Task<List<int>> GetPlayerIds(IPlayerFilterParameter parameters, CancellationToken cancellationToken);

        Task<Dictionary<int, PlayerInfo>> GetPlayerInfo(IList<int> playerIds, CancellationToken cancellationToken);

        Task<Dictionary<int, PlayerPopulationHistory>> GetPlayerPopulationHistory(IList<int> playerIds, PlayerContainsPopulationHistoryParameters parameters, CancellationToken cancellationToken);

        IEnumerable<PlayerDto> GetPlayers(IList<int> playerIds);

        Task<Dictionary<int, PlayerRecord>> GetRecords(IList<int> villageIds, CancellationToken cancellationToken);
    }
}