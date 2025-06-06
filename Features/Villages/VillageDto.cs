namespace Features.Villages
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