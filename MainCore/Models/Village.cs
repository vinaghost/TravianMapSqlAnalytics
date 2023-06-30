using System.Text.Json.Serialization;

namespace MainCore.Models
{
    public class Village
    {
        public Village()
        {
        }

        public Village(int mapId, int x, int y, int tribe, int id, string name, int playerId, string playerName, int allyId, string allyName, int pop, string region, bool isCapital, bool isCity, int victoryPoints)
        {
            MapId = mapId;
            X = x;
            Y = y;
            Tribe = tribe;
            Id = id;
            Name = name;
            PlayerId = playerId;
            PlayerName = playerName;
            AllyId = allyId;
            AllyName = allyName;
            Pop = pop;
            Region = region;
            IsCapital = isCapital;
            IsCity = isCity;
            VictoryPoints = victoryPoints;
        }

        public int MapId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Tribe { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public int AllyId { get; set; }
        public string AllyName { get; set; }
        public int Pop { get; set; }
        public string Region { get; set; }
        public bool IsCapital { get; set; }
        public bool IsCity { get; set; }
        public int VictoryPoints { get; set; }

        [JsonIgnore]
        public Coordinates Coordinates { get => new(X, Y); }
    }
}