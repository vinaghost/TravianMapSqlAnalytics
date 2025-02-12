using FastEndpoints;

namespace WebAPI.Groups
{
    public class Population : SubGroup<ServerUrlBased>
    {
        public Population()
        {
            Configure(
                "populations",
                ep =>
                {
                    ep.Description(x => x.WithDescription("Populations related"));
                });
        }
    }
}