﻿namespace Features.Shared.Dtos
{
    public record PlayerDto(int PlayerId, string PlayerName, int AllianceId, string AllianceName, int Population, int VillageCount, int Tribe);
}