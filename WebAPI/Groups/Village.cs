using FastEndpoints;

namespace WebAPI.Groups
{
    public class Village : SubGroup<ServerUrlBased>
    {
        public Village()
        {
            Configure(
                "villages",
                ep =>
                {
                    ep.Description(x => x.WithDescription("Villages related"));
                });
        }
    }
}