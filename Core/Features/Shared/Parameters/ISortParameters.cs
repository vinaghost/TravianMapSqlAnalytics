namespace Core.Features.Shared.Parameters
{
    public interface ISortParameters
    {
        int SortOrder { get; }
        string SortField { get; }
    }
}