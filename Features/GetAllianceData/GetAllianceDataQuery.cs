using Features.Shared.Query;

namespace Features.GetAllianceData
{
    public record GetAllianceDataQuery(int AllianceId) : ICachedQuery<AllianceDataDto?>
    {
        public string CacheKey => $"{nameof(GetAllianceDataQuery)}_{AllianceId}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }
}