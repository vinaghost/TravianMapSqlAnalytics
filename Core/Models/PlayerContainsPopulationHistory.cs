﻿namespace Core.Models
{
    public record PlayerContainsPopulationHistory(
        int AllianceId,
        string AllianceName,
        int PlayerId,
        string PlayerName,
        int ChangePopulation,
        IList<PopulationHistoryRecord> Populations);
}