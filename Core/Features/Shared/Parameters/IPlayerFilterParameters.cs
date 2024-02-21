namespace Core.Features.Shared.Parameters
{
    public interface IPlayerFilterParameters
    {
        int MinPlayerPopulation { get; }
        int MaxPlayerPopulation { get; }

        IList<int> Alliances { get; }
        IList<int> ExcludeAlliances { get; }
    }
}