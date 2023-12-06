using Core;
using MapSqlAspNetCoreMVC.Models;
using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using MediatR;
using X.PagedList;

namespace MapSqlAspNetCoreMVC.CQRS.Queries
{
    public record GetVillageByInputQuery(VillageInput Input) : IRequest<IPagedList<Village>>;

    public class GetVillageByVillageInputQueryHandler : IRequestHandler<GetVillageByInputQuery, IPagedList<Village>>
    {
        private readonly ServerDbContext _context;

        public GetVillageByVillageInputQueryHandler(ServerDbContext context)
        {
            _context = context;
        }

        public async Task<IPagedList<Village>> Handle(GetVillageByInputQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var input = request.Input;

            var query = _context.Villages
                .Where(x => x.Population >= input.MinPop);
            if (input.MaxPop != -1 && input.MaxPop > input.MinPop)
            {
                query = query.Where(x => x.Population <= input.MaxPop);
            }
            if (input.TribeId != 0)
            {
                query = query.Where(x => x.Tribe == input.TribeId);
            }

            var getVillageQuery = query.Join(_context.Players, x => x.PlayerId, x => x.PlayerId, (village, player) => new
            {
                village.VillageId,
                player.AllianceId,
                VillageName = village.Name,
                PlayerName = player.Name,
                TribeId = village.Tribe,
                village.X,
                village.Y,
                village.Population,
                village.IsCapital,
            });

            if (input.IgnoreCapital)
            {
                getVillageQuery = getVillageQuery.Where(x => x.IsCapital == false);
            }

            if (input.IgnoreNormalVillage)
            {
                getVillageQuery = getVillageQuery.Where(x => x.IsCapital == true);
            }

            var getAllianceQuery = getVillageQuery.Join(_context.Alliances, x => x.AllianceId, x => x.AllianceId, (village, alliance) => new
            {
                village.VillageId,
                village.AllianceId,
                AllianceName = alliance.Name,
                village.PlayerName,
                village.VillageName,
                village.TribeId,
                village.X,
                village.Y,
                village.Population,
                village.IsCapital,
            });

            if (input.AllianceId != -1)
            {
                getAllianceQuery = getAllianceQuery.Where(x => x.AllianceId == input.AllianceId);
            }

            var centerCoordinate = new Coordinates(input.X, input.Y);
            var result = getAllianceQuery
                .AsEnumerable()
                .Select(x => new Village
                {
                    VillageId = x.VillageId,
                    VillageName = x.VillageName,
                    PlayerName = x.PlayerName,
                    AllianceName = x.AllianceName,
                    Tribe = Constants.TribeNames[x.TribeId],
                    X = x.X,
                    Y = x.Y,
                    Population = x.Population,
                    IsCapital = x.IsCapital,
                    Distance = centerCoordinate.Distance(new Coordinates(x.X, x.Y)),
                })
                .OrderBy(x => x.Distance)
                .ToPagedList(input.PageNumber, input.PageSize);
            return result;
        }
    }
}