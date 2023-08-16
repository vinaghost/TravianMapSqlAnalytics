namespace MapSqlQuery.Models.Input
{
    public class PlayerLookupInput
    {
        public string PlayerName { get; set; }
        public int Days { get; set; } = 7;
    }
}