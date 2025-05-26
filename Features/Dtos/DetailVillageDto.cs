using Features.Enums;

namespace Features.Dtos
{
    public record DetailVillageDto(int AllianceId,
                             string AllianceName,
                             int PlayerId,
                             string PlayerName,
                             int VillageId,
                             int MapId,
                             string VillageName,
                             int X,
                             int Y,
                             bool IsCapital,
                             Tribe Tribe,
                             int Population,
                             double Distance);
}