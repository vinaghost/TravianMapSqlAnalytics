namespace Core.Features.Shared.Parameters
{
    internal interface ISortParameters
    {
        int SortOrder { get; }
        string SortField { get; }
    }
}