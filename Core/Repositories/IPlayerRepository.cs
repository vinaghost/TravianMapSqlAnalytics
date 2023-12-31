﻿using Core.Models;
using Core.Parameters;

namespace Core.Repositories
{
    public interface IPlayerRepository
    {
        IEnumerable<PlayerContainsPopulationHistory> GetPlayers(PlayerContainsPopulationHistoryParameters parameters);
        IEnumerable<PlayerContainsAllianceHistory> GetPlayers(PlayerContainsAllianceHistoryParameters parameters);
        IEnumerable<Player> GetPlayers(PlayerParameters parameters);
        Task<Dictionary<int, PlayerRecord>> GetRecords(List<int> PlayersId, CancellationToken cancellationToken);
    }
}