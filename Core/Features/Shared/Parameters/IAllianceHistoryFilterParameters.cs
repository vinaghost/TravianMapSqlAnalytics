namespace Core.Features.Shared.Parameters
{
    public interface IAllianceHistoryFilterParameters : IHistoryParameters
    {
        int MinChangeAlliance { get; }
        int MaxChangeAlliance { get; }
    }
}