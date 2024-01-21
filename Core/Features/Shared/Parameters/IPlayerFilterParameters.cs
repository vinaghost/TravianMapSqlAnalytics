namespace Core.Features.Shared.Parameters
{
    public interface IPlayerFilterParameters
    {
        List<int> Alliances { get; }
        List<int> Players { get; }
    }
}