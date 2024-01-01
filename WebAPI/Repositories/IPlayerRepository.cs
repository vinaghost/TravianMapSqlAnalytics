﻿using WebAPI.Models.Output;
using WebAPI.Models.Parameters;

namespace WebAPI.Repositories
{
    public interface IPlayerRepository
    {
        IQueryable<Core.Models.Player> GetQueryable(IPlayerFilterParameter parameters);
        Task<Dictionary<int, RecordPlayer>> GetRecords(List<int> PlayersId, CancellationToken cancellationToken);
    }
}