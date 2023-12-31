﻿namespace Core.Models
{
    public record PlayerContainsPopulationHistory(
        int AllianceId,
        int PlayerId,
        string PlayerName,
        int ChangePopulation,
        IList<PopulationHistoryRecord> Populations);
}