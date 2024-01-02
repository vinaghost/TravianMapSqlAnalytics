namespace Core.Entities
{
    public class Server
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Zone { get; set; }
        public DateTime StartDate { get; set; }

        public int AllianceCount { get; set; }
        public int PlayerCount { get; set; }
        public int VillageCount { get; set; }
        public int OasisCount { get; set; }

        public override string ToString()
        {
            return $"{Zone} - {Url} - {StartDate} - Alliance: {AllianceCount} - Player: {PlayerCount} - Village: {VillageCount} - Oasis: {OasisCount}";
        }
    }
}