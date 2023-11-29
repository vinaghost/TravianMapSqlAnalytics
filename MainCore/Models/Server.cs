namespace MainCore.Models
{
    public class Server
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public DateTime Start { get; set; }
        public string Zone { get; set; }
        public bool IsClosed { get; set; }
        public bool IsEnded { get; set; }
    }
}