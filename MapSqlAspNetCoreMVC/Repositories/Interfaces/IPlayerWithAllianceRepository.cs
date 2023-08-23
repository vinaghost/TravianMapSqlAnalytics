﻿using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;

namespace MapSqlAspNetCoreMVC.Repositories.Interfaces
{
    public interface IPlayerWithAllianceRepository : IRepository<PlayerWithAllianceInput, List<PlayerWithAlliance>>
    {
    }
}