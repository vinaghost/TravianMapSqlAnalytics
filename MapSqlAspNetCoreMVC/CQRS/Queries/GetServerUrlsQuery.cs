using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MapSqlAspNetCoreMVC.CQRS.Queries
{
    public record GetServerUrlsQuery : IRequest<List<string>>;

    public class GetUrlServerUrlsQueryHandler : IRequestHandler<GetServerUrlsQuery, List<string>>
    {
        private readonly ServerListDbContext _context;

        public GetUrlServerUrlsQueryHandler(ServerListDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> Handle(GetServerUrlsQuery request, CancellationToken cancellationToken)
        {
            var servers = await _context.Servers
                .Select(x => x.Url)
                .OrderBy(x => x)
                .ToListAsync(cancellationToken: cancellationToken);
            return servers;
        }
    }
}