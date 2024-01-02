namespace Core.Config
{
    public class DatabaseOption
    {
        public const string Position = "Database";
        public string Host { get; set; } = "";
        public int Port { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}