using MainCore.Models;
using Riok.Mapperly.Abstractions;
using System.Text.Json.Serialization;

namespace MapSqlDatabaseUpdate.Models
{
    public class ServerRaw
    {
        public int Id { get; set; }
        public string Server { get; set; }
        public long Start { get; set; }
        public DateTime StartDate => new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Start);

        [JsonPropertyName("from_zone")]
        public string Zone { get; set; }

        public int Closed { get; set; }
        public bool IsClosed => Closed == 1;
        public int Ended { get; set; }
        public bool IsEnded => Ended == 1;
    }

    [Mapper]
    public static partial class ServerMapper
    {
        [MapProperty(nameof(ServerRaw.StartDate), nameof(Server.Start))]
        [MapProperty(nameof(ServerRaw.Server), nameof(Server.Url))]
        public static partial Server ToServer(this ServerRaw serverRaw);
    }
}