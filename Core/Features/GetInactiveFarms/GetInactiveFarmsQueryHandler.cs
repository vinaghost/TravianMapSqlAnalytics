using Core.Features.Shared.Dtos;
using Core.Features.Shared.Models;
using MediatR;
using X.PagedList;

namespace Core.Features.GetInactiveFarms
{
    public class GetInactiveFarmsQueryHandler(ServerDbContext dbContext) : IRequestHandler<GetInactiveFarmsQuery, IPagedList<InactiveFarmDto>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<IPagedList<InactiveFarmDto>> Handle(GetInactiveFarmsQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;

            var inactivePlayers = _dbContext.PlayerPopulationHistory
                .Where(x => x.Date >= parameters.Date)
                .GroupBy(x => x.PlayerId)
                .Where(x => x.Select(x => x.Change).Max() == 0 && x.Select(x => x.Change).Min() == 0)
                .Select(x => x.Key);

            var filterInactivePlayers = _dbContext.Players
                .Where(x => inactivePlayers.Contains(x.Id));

            if (parameters.MaxPlayerPopulation != 0)
            {
                filterInactivePlayers = filterInactivePlayers
                    .Where(x => x.Population >= parameters.MinPlayerPopulation)
                    .Where(x => x.Population <= parameters.MaxPlayerPopulation);
            }

            var filterVillages = _dbContext.Villages
                .AsQueryable();

            if (parameters.MaxVillagePopulation != 0)
            {
                filterVillages = filterVillages
                    .Where(x => x.Population >= parameters.MinVillagePopulation)
                    .Where(x => x.Population <= parameters.MaxVillagePopulation);
            }

            var filterPopulations = _dbContext.VillagePopulationHistory
                .Where(x => x.Date >= parameters.Date);

            var villages = filterVillages
                .Join(filterInactivePlayers,
                    x => x.PlayerId,
                    x => x.Id,
                    (village, player) => new
                    {
                        Player = new PlayerDto(player.Id, player.Name, player.Population, player.VillageCount),
                        Village = new VillageDto(village.MapId, village.Name, village.X, village.Y, village.Population),
                        VillageId = village.Id,
                    })
                .GroupJoin(filterPopulations,
                    x => x.VillageId,
                    x => x.VillageId,
                    (village, populations) => new
                    {
                        village.Player,
                        village.Village,
                        Populations = populations
                            .OrderByDescending(x => x.Date)
                            .Select(x => new PopulationDto(x.Date, x.Population, x.Change))
                            .ToList(),
                    })
                .AsEnumerable();

            var centerCoordinate = new Coordinates(parameters.X, parameters.Y);

            var dtos = villages
                .Select(x => new InactiveFarmDto()
                {
                    Distance = centerCoordinate.Distance(new Coordinates(x.Village.X, x.Village.Y)),
                    Player = x.Player,
                    Village = x.Village,
                    Populations = x.Populations
                });

            if (parameters.MaxDistance != 0)
            {
                dtos = dtos
                    .Where(x => x.Distance >= parameters.MinDistance)
                    .Where(x => x.Distance <= parameters.MaxDistance);
            }

            var orderDtos = dtos
                .OrderBy(x => x.Distance);

            return await orderDtos
                .ToPagedListAsync(parameters.PageNumber, parameters.PageSize);
        }
    }
}