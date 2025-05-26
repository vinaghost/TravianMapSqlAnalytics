using Features.Enums;

namespace Features.Dtos
{
    public record VillageDto(int MapId,
                             int VillageId,
                             string VillageName,
                             int X,
                             int Y,
                             Tribe Tribe,
                             int Population,
                             bool IsCapital);
}