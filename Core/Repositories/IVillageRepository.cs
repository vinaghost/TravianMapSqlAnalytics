﻿using Core.Models;
using Core.Parameters;

namespace Core.Repositories
{
    public interface IVillageRepository
    {
        IEnumerable<VillageContainsDistance> GetVillages(VillageParameters parameters);

        IEnumerable<VillageContainPopulationHistory> GetVillages(VillageContainsPopulationHistoryParameters parameters);
    }
}