namespace WebAPI.Models.Parameters
{
    public class PlayerParameters : IPaginationParameters, IPlayerFilterParameter
    {
        public int PageNumber { get; set; } = DefaultParameters.PageNumber;
        public int PageSize { get; set; } = DefaultParameters.PageSize;

        public List<int> Alliances { get; set; } = [];
        public List<int> Players { get; set; } = [];

        public void Deconstruct(
            out List<int> alliances,
            out List<int> players
        ) => (alliances, players) = (Alliances, Players);
    }
}