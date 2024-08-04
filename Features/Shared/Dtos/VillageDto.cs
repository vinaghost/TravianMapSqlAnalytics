using Features.Shared.Enums;

namespace Features.Shared.Dtos
{
    public record VillageDto(int PlayerId,
                              int AllianceId,
                              int MapId,
                              string VillageName,
                              int X,
                              int Y,
                              bool IsCapital,
                              Tribe Tribe,
                              int Population,
                              double Distance);
}