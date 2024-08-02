using Features.Shared.Enums;

namespace Features.Villages
{
    public record VillageDto(int PlayerId,
                              string PlayerName,
                              int AllianceId,
                              string AllianceName,
                              int MapId,
                              string VillageName,
                              int X,
                              int Y,
                              bool IsCapital,
                              Tribe Tribe,
                              int Population,
                              double Distance);
}