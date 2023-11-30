namespace MapSqlDatabaseUpdate.Commands
{
    public class ServerCommand
    {
        public ServerCommand(string serverUrl)
        {
            ServerUrl = serverUrl;
        }

        public string ServerUrl { get; }
    }
}