using MediatR;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using WebAPI.Repositories;
using X.PagedList;

namespace WebAPI.Queries
{
    public record GetServerQuery(ServerParameters Parameters) : ICachedQuery<IPagedList<Server>>
    {
        public string CacheKey => $"{nameof(GetServerQuery)}_{Parameters.Key}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class GetServerQueryHandler(ServerRepository serverRepository) : IRequestHandler<GetServerQuery, IPagedList<Server>>
    {
        private readonly ServerRepository _serverRepository = serverRepository;

        public async Task<IPagedList<Server>> Handle(GetServerQuery request, CancellationToken cancellationToken)
        {
            return await _serverRepository.GetServers(request.Parameters);
        }
    }
}