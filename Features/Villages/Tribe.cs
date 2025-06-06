using System.Text.Json.Serialization;

namespace Features.Villages
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Tribe
    {
        All,
        Romans,
        Teutons,
        Gauls,
        Egyptians = 6,
        Huns,
        Spartans,
    }
}