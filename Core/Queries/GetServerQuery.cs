using Core.Models;
using Core.Parameters;
using Core.Repositories;
using MediatR;
using X.PagedList;

namespace Core.Queries
{
    public record GetServerQuery(ServerParameters Parameters) : ICachedQuery<IPagedList<Server>>
    {
        public string CacheKey => $"{nameof(GetServerQuery)}_{Parameters.Key}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class GetServerQueryHandler(IServerRepository serverRepository) : IRequestHandler<GetServerQuery, IPagedList<Server>>
    {
        private readonly IServerRepository _serverRepository = serverRepository;

        public async Task<IPagedList<Server>> Handle(GetServerQuery request, CancellationToken cancellationToken)
        {
            return await _serverRepository.GetServers(request.Parameters);
        }
    }
}