using Features.Shared.Query;
using FluentResults;

namespace Features.GetPlayerData
{
    public record GetPlayerDataQuery(int PlayerId) : ICachedQuery<Result<PlayerDataDto>>
    {
        public string CacheKey => $"{nameof(GetPlayerDataQuery)}_{PlayerId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }
}