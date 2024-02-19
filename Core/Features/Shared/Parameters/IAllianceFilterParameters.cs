namespace Core.Features.Shared.Parameters
{
    public interface IAllianceFilterParameters
    {
        IList<int> Alliances { get; }
        IList<int> ExcludeAlliances { get; }
    }
}