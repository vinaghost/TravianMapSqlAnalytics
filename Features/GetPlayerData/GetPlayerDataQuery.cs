using Features.Shared.Query;

namespace Features.GetPlayerData
{
    public record GetPlayerDataQuery(int PlayerId) : ICachedQuery<PlayerDataDto?>
    {
        public string CacheKey => $"{nameof(GetPlayerDataQuery)}_{PlayerId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }
}