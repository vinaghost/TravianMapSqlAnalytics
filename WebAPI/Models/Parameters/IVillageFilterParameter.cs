﻿namespace WebAPI.Models.Parameters
{
    public interface IVillageFilterParameter : IPopulationFilterParameter
    {
        List<int> Alliances { get; }
        List<int> Players { get; }
        List<int> Villages { get; }
    }
}