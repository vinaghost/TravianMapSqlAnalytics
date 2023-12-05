using Core;
using MapSqlAspNetCoreMVC.Models;
using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using MediatR;

namespace MapSqlAspNetCoreMVC.CQRS.Queries
{
    public record GetVillageByInputQuery(VillageInput Input) : IRequest<List<Village>>;

    public class GetVillageByVillageInputQueryHandler : IRequestHandler<GetVillageByInputQuery, List<Village>>
    {
        private readonly ServerDbContext _context;

        public GetVillageByVillageInputQueryHandler(ServerDbContext context)
        {
            _context = context;
        }

        public async Task<List<Village>> Handle(GetVillageByInputQuery request, CancellationToken cancellationToken)
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

            var result = getAllianceQuery.AsEnumerable();

            var villagesInfo = new List<Village>();
            var centerCoordinate = new Coordinates(input.X, input.Y);

            foreach (var village in result)
            {
                var villageCoordinate = new Coordinates(village.X, village.Y);
                var distance = centerCoordinate.Distance(villageCoordinate);

                var villageInfo = new Village
                {
                    VillageId = village.VillageId,
                    VillageName = village.VillageName,
                    PlayerName = village.PlayerName,
                    AllianceName = village.AllianceName,
                    Tribe = Constants.TribeNames[village.TribeId],
                    X = village.X,
                    Y = village.Y,
                    Population = village.Population,
                    IsCapital = village.IsCapital,
                    Distance = distance,
                };
                villagesInfo.Add(villageInfo);
            }

            var oredered = villagesInfo.OrderBy(x => x.Distance).ToList();
            return oredered;
        }
    }
}