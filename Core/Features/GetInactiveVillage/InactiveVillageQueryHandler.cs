using Core.Features.Shared.Dtos;
using Core.Features.Shared.Models;
using MediatR;
using X.PagedList;

namespace Core.Features.GetInactiveVillage
{
    public class InactiveVillageQueryHandler(ServerDbContext dbContext) : IRequestHandler<InactiveVillageQuery, IPagedList<InactiveVillageDto>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<IPagedList<InactiveVillageDto>> Handle(InactiveVillageQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;

            var changePopulationVillages = _dbContext.VillagesPopulations
                .Where(x => x.Date >= parameters.Date)
                .GroupBy(x => x.VillageId)
                .Where(x => Math.Abs(
                    x.OrderBy(x => x.VillageId).Select(x => x.Population).Max()
                    -
                    x.OrderBy(x => x.VillageId).Select(x => x.Population).Average())
                    >= 0.1)
                .Select(x => x.Key);

            var notChangePopulationPlayers = _dbContext.Villages
                .GroupBy(x => x.PlayerId)
                .Where(x => x.OrderBy(x => x.VillageId).Select(x => x.Population).Sum() >= parameters.MinPlayerPopulation)
                .Where(x => x.OrderBy(x => x.VillageId).Select(x => x.Population).Sum() <= parameters.MaxPlayerPopulation)
                .Where(x => x.OrderBy(x => x.VillageId).Any(x => changePopulationVillages.Contains(x.VillageId)))
                .Select(x => x.Key);

            var villages = _dbContext.Villages
                .Where(x => notChangePopulationPlayers.Contains(x.PlayerId))
                .Where(x => x.Population >= parameters.MinVillagePopulation)
                .Where(x => x.Population <= parameters.MaxVillagePopulation)
                .Select(x => new
                {
                    x.PlayerId,
                    Village = new VillageDto(x.MapId, x.Name, x.X, x.Y, x.Population),
                    Populations = x.Populations
                        .Where(x => x.Date <= parameters.Date)
                        .OrderByDescending(x => x.Date)
                        .Select(x => new PopulationDto(x.Date, x.Population))
                });

            var players = _dbContext.Players
                .Join(villages,
                    x => x.PlayerId,
                    x => x.PlayerId,
                    (player, village) => new
                    {
                        Player = new PlayerDto(player.PlayerId, player.Name, player.Villages.Select(x => x.Population).Sum(), player.Villages.Count()),
                        village.Village,
                        Populations = village.Populations.ToList()
                    })
                .AsEnumerable();

            var centerCoordinate = new Coordinates(parameters.X, parameters.Y);

            var distance = players
                .Select(x => new InactiveVillageDto()
                {
                    Distance = centerCoordinate.Distance(new Coordinates(x.Village.X, x.Village.Y)),
                    Player = x.Player,
                    Village = x.Village,
                    Populations = x.Populations
                });

            var filter = distance
                .Where(x => x.Distance >= request.Parameters.MinDistance)
                .Where(x => x.Distance <= request.Parameters.MaxDistance);

            var order = distance
                .OrderBy(x => x.Distance);

            return await order
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);
        }
    }
}