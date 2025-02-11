using FastEndpoints;
using WebAPI.EndpointFilters;

namespace WebAPI.Groups
{
    public class ServerUrlBased : Group
    {
        public ServerUrlBased()
        {
            Configure(
                "servers/{serverUrl}",
                ep =>
                {
                    ep.Description(x => x.WithDescription("Required server url"));
                    ep.Options(x => x.AddEndpointFilter<ServerUrlFilter>());
                });
        }
    }
}