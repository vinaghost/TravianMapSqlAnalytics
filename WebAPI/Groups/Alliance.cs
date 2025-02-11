using FastEndpoints;

namespace WebAPI.Groups
{
    public class Alliance : SubGroup<ServerUrlBased>
    {
        public Alliance()
        {
            Configure(
                "alliances",
                ep =>
                {
                    ep.Description(x => x.WithDescription("Alliances related"));
                });
        }
    }
}