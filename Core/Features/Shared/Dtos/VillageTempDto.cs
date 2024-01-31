namespace Core.Features.Shared.Dtos
{
    public record VillageTempDto(int PlayerId, int VillageId, int MapId, string VillageName, int X, int Y, int Population, bool IsCapital, int Tribe);
}