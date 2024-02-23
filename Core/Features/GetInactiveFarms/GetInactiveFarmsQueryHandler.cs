using Core.Entities;
using Core.Features.Shared.Dtos;
using Core.Features.Shared.Handler;
using Core.Features.Shared.Models;
using Core.Features.Shared.Parameters;
using MediatR;
using X.PagedList;

namespace Core.Features.GetInactiveFarms
{
    public class GetInactiveFarmsQueryHandler(ServerDbContext dbContext) : VillageDataQueryHandler(dbContext), IRequestHandler<GetInactiveFarmsQuery, IPagedList<VillageDataDto>>
    {
        public async Task<IPagedList<VillageDataDto>> Handle(GetInactiveFarmsQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;

            var players = GetInactivePlayers(parameters);
            var villages = GetVillages(parameters, parameters);

            var populations = GetPopulation();

            var data = players
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
                        Player = new PlayerDto(player.PlayerId, player.PlayerName, player.AllianceId, player.AllianceName, player.Population, player.VillageCount, village.Tribe),
                        Village = new VillageDto(village.MapId, village.Name, village.X, village.Y, village.Population, village.IsCapital),
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

            var dtos = data
                .Select(x => new VillageDataDto(centerCoordinate.Distance(x.Village.X, x.Village.Y), x.Player, x.Village, x.Populations));

            var orderDtos = dtos
                .OrderBy(x => x.Distance);

            return await ToPagedList(orderDtos, parameters);
        }

        private static bool IsPlayerFiltered(IPlayerFilterParameters parameters)
        {
            if (parameters.Alliances.Count > 0) return true;
            if (parameters.ExcludeAlliances.Count > 0) return true;
            if (parameters.MaxPlayerPopulation != 0) return true;
            return false;
        }

        private IQueryable<int> GetInactivePlayerIds(InactiveFarmParameters parameters)
        {
            if (IsPlayerFiltered(parameters))
            {
                var query = GetPlayers(parameters)
                    .Join(_dbContext.PlayerPopulationHistory
                            .Where(x => x.Date >= DefaultParameters.Date),
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
                   .Where(x => x.Date >= DefaultParameters.Date)
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
    }
}