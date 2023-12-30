namespace WebAPI.Models.Parameters
{
    public class PlayerParameters : IPaginationParameters, IPlayerFilterParameter
    {
        public int PageNumber { get; } = DefaultParameters.PageNumber;
        public int PageSize { get; } = DefaultParameters.PageSize;

        public List<int> Alliances { get; } = [];
        public List<int> Players { get; } = [];

        public void Deconstruct(
            out List<int> alliances,
            out List<int> players
        ) => (alliances, players) = (Alliances, Players);
    }
}