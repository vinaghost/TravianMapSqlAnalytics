﻿namespace Features.Shared.Dtos
{
    public record PlayerDto(int AllianceId,
                            string AllianceName,
                            int PlayerId,
                            string PlayerName,
                            int VillageCount,
                            int Population);
}