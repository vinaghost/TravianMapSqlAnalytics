namespace Core.Parameters
{
    public interface IAllianceHistoryFilterParameter : IHistoryParameters
    {
        int MinChangeAlliance { get; }
        int MaxChangeAlliance { get; }
    }
}