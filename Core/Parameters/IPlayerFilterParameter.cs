namespace Core.Parameters
{
    public interface IPlayerFilterParameter
    {
        List<int> Alliances { get; }
        List<int> Players { get; }
    }
}