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

            var players = _dbContext.Players
                .AsQueryable();
            var villages = _dbContext.Villages
                .AsQueryable();
            var population = _dbContext.VillagePopulationHistory
                .Where(x => x.Date >= parameters.Date);

            if (parameters.MaxPlayerPopulation != 0)
            {
                players = players
                    .Where(x => x.Population >= parameters.MinPlayerPopulation)
                    .Where(x => x.Population <= parameters.MaxPlayerPopulation);
            }

            var query = villages
                .Join(population,
                    x => x.Id,
                    x => x.VillageId,
                    (village, population) => new
                    {
                        village.PlayerId,
                        population.Change,
                    })
                .GroupBy(x => x.PlayerId)
                .Where(x =>
                    x.OrderBy(x => x.PlayerId).MaxBy(x => x.Change).Change
                    -
                    x.OrderBy(x => x.PlayerId).MinBy(x => x.Change).Change
                    == 0)
                .Select(x => new
                {
                    PlayerId = x.Key,
                });

            var data = query
                .Select(x => new
                {
                    w
                    x.Data
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