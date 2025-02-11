using FastEndpoints;

namespace WebAPI.Groups
{
    public class Player : SubGroup<ServerUrlBased>
    {
        public Player()
        {
            Configure(
                "players",
                ep =>
                {
                    ep.Description(x => x.WithDescription("Players related"));
                });
        }
    }
}