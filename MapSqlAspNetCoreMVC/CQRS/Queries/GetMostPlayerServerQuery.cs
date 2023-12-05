using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MapSqlAspNetCoreMVC.CQRS.Queries
{
    public record GetMostPlayerServerQuery : IRequest<string>;

    public class GetMostPlayerServerQueryHandler : IRequestHandler<GetMostPlayerServerQuery, string>
    {
        private readonly ServerListDbContext _context;

        public GetMostPlayerServerQueryHandler(ServerListDbContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(GetMostPlayerServerQuery request, CancellationToken cancellationToken)
        {
            return await _context.Servers.OrderByDescending(x => x.PlayerCount).Select(x => x.Url).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }
    }
}