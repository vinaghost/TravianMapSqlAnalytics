using MapSqlQuery.Models.Input;
using MapSqlQuery.Models.Output;

namespace MapSqlQuery.Models.View
{
    public class PlayerLookupViewModel : IServerViewModel
    {
        public PlayerLookupInput Input { get; set; }
        public PlayerInfo Player { get; set; }
        public List<DateTime> Dates { get; set; }
        public string Server { get; set; }
    }
}