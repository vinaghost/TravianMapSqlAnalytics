namespace Core.Features.Shared.Parameters
{
    public interface IAllianceHistoryFilterParameter : IHistoryParameters
    {
        int MinChangeAlliance { get; }
        int MaxChangeAlliance { get; }
    }
}