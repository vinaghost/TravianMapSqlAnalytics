using Core.Models;
using Core.Repositories;
using MediatR;

namespace Core.Queries
{
    public record GetServerUrlQuery : ICachedQuery<IEnumerable<ServerRecord>>
    {
        public string CacheKey => $"{nameof(GetServerUrlQuery)}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class GetServerUrlQueryHandler(IServerRepository serverRepository) : IRequestHandler<GetServerUrlQuery, IEnumerable<ServerRecord>>
    {
        private readonly IServerRepository _serverRepository = serverRepository;

        public async Task<IEnumerable<ServerRecord>> Handle(GetServerUrlQuery request, CancellationToken cancellationToken)
        {
            var servers = await _serverRepository.GetServerRecords(cancellationToken);
            return servers;
        }
    }
}