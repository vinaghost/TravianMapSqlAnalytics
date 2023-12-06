using Core;
using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using MediatR;
using X.PagedList;

namespace MapSqlAspNetCoreMVC.CQRS.Queries
{
    public record GetServersQuery(HomeInput Input) : IRequest<IPagedList<Server>>;

    public class GetServersQueryHandler : IRequestHandler<GetServersQuery, IPagedList<Server>>
    {
        private readonly ServerListDbContext _context;

        public GetServersQueryHandler(ServerListDbContext context)
        {
            _context = context;
        }

        public async Task<IPagedList<Server>> Handle(GetServersQuery request, CancellationToken cancellationToken)
        {
            var servers = _context.Servers
                .OrderByDescending(x => x.PlayerCount)
                .Select(x => new Server
                {
                    Url = x.Url,
                    Region = x.Zone,
                    StartDate = x.StartDate,
                    AllianceCount = x.AllianceCount,
                    PlayerCount = x.AllianceCount,
                    VillageCount = x.VillageCount,
                });
            return await servers.ToPagedListAsync(request.Input.PageNumber, request.Input.PageSize);
        }
    }
}