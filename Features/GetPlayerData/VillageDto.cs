﻿using Features.Shared.Dtos;

namespace Features.GetPlayerData
{
    public record VillageDto(int MapId, string VillageName, int X, int Y, int Population, bool IsCapital, int Tribe, IList<PopulationDto> Populations);
}