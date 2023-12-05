using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MapSqlAspNetCoreMVC.CQRS.Queries
{
    public record GetServersQuery : IRequest<List<string>>;

    public class GetServersQueryHandler : IRequestHandler<GetServersQuery, List<string>>
    {
        private readonly ServerListDbContext _context;

        public GetServersQueryHandler(ServerListDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> Handle(GetServersQuery request, CancellationToken cancellationToken)
        {
            return await _context.Servers.OrderByDescending(x => x.PlayerCount).Select(x => x.Url).ToListAsync(cancellationToken: cancellationToken);
        }
    }
}