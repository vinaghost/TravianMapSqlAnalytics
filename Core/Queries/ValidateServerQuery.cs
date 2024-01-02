using Core.Repositories;
using MediatR;

namespace Core.Queries
{
    public record ValidateServerQuery(string Server) : ICachedQuery<bool>
    {
        public string CacheKey => $"Validate_{Server}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class ValidateServerQueryHandler(IServerRepository serverRepository) : IRequestHandler<ValidateServerQuery, bool>
    {
        private readonly IServerRepository _serverRepository = serverRepository;

        public async Task<bool> Handle(ValidateServerQuery request, CancellationToken cancellationToken)
        {
            return await _serverRepository.Validate(request.Server, cancellationToken);
        }
    }
}