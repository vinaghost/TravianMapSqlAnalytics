using Core.Entities;
using Core.Features.Shared.Dtos;
using Core.Features.Shared.Models;
using MediatR;
using X.PagedList;

namespace Core.Features.GetInactiveFarms
{
    public class GetInactiveFarmsQueryHandler(ServerDbContext dbContext) : IRequestHandler<GetInactiveFarmsQuery, IPagedList<VillageDataDto>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<IPagedList<VillageDataDto>> Handle(GetInactiveFarmsQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;

            var villages = GetVillages(parameters);

            var populations = GetPopulation(parameters);

            var data = GetInactivePlayers(parameters)
                .Join(_dbContext.Alliances,
                    x => x.AllianceId,
                    x => x.Id,
                    (player, alliance) => new
                    {
                        PlayerId = player.Id,
                        PlayerName = player.Name,
                        AllianceId = alliance.Id,
                        AllianceName = alliance.Name,
                        player.Population,
                        player.VillageCount
                    })
                .Join(villages,
                    x => x.PlayerId,
                    x => x.PlayerId,
                    (player, village) => new
                    {
                        Player = new PlayerDto(player.PlayerId, player.PlayerName, player.AllianceId, player.AllianceName, player.Population, player.VillageCount),
                        Village = new VillageDto(village.MapId, village.Name, village.X, village.Y, village.Population, village.Tribe),
                        VillageId = village.Id,
                    })
                .GroupJoin(populations,
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

            if (parameters.MaxDistance != 0)
            {
                data = data
                    .Where(x => new Coordinates(x.Village.X, x.Village.Y).InSimpleRange(centerCoordinate, parameters.MaxDistance));
            }

            var dtos = data
                .Select(x => new VillageDataDto(centerCoordinate.Distance(new Coordinates(x.Village.X, x.Village.Y)), x.Player, x.Village, x.Populations));

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

        private static bool IsPlayerFiltered(InactiveFarmParameters parameters)
        {
            if (parameters.Alliances.Count > 0) return true;
            if (parameters.ExcludeAlliances.Count > 0) return true;
            if (parameters.MaxPlayerPopulation != 0) return true;
            return false;
        }

        private IQueryable<Player> GetPlayers(InactiveFarmParameters parameters)
        {
            var query = _dbContext.Players
                .AsQueryable();
            if (parameters.Alliances.Count > 0)
            {
                query = query
                    .Where(x => parameters.Alliances.Contains(x.AllianceId));
            }
            else if (parameters.ExcludeAlliances.Count > 0)
            {
                query = query
                    .Where(x => !parameters.ExcludeAlliances.Contains(x.AllianceId));
            }

            if (parameters.MaxPlayerPopulation != 0)
            {
                query = query
                    .Where(x => x.Population >= parameters.MinPlayerPopulation)
                    .Where(x => x.Population <= parameters.MaxPlayerPopulation);
            }
            return query;
        }

        private IQueryable<int> GetInactivePlayerIds(InactiveFarmParameters parameters)
        {
            if (IsPlayerFiltered(parameters))
            {
                var query = GetPlayers(parameters)
                    .Join(_dbContext.PlayerPopulationHistory
                            .Where(x => x.Date >= parameters.Date),
                        x => x.Id,
                        x => x.PlayerId,
                        (player, population) => new
                        {
                            player.Id,
                            population.Change
                        })
                    .GroupBy(x => x.Id)
                    .Where(x => x.Count() >= 7 && x.Select(x => x.Change).Max() == 0 && x.Select(x => x.Change).Min() == 0)
                    .Select(x => x.Key);
                return query;
            }
            else
            {
                var query = _dbContext.PlayerPopulationHistory
                   .Where(x => x.Date >= parameters.Date)
                   .GroupBy(x => x.PlayerId)
                   .Where(x => x.Count() >= 7 && x.Select(x => x.Change).Max() == 0 && x.Select(x => x.Change).Min() == 0)
                   .Select(x => x.Key);
                return query;
            }
        }

        private IQueryable<Player> GetInactivePlayers(InactiveFarmParameters parameters)
        {
            var ids = GetInactivePlayerIds(parameters);

            var loaded = ids.ToList();

            return _dbContext.Players
                .Where(x => ids.Contains(x.Id));
        }

        private IQueryable<Village> GetVillages(InactiveFarmParameters parameters)
        {
            var query = _dbContext.Villages
               .AsQueryable();

            if (parameters.MaxVillagePopulation != 0)
            {
                query = query
                    .Where(x => x.Population >= parameters.MinVillagePopulation)
                    .Where(x => x.Population <= parameters.MaxVillagePopulation);
            }
            return query;
        }

        private IQueryable<VillagePopulationHistory> GetPopulation(InactiveFarmParameters parameters)
        {
            var query = _dbContext.VillagePopulationHistory
                .Where(x => x.Date >= parameters.Date);
            return query;
        }
    }
}