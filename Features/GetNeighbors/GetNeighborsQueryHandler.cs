using Core;
using Features.Shared.Dtos;
using Features.Shared.Handler;
using Features.Shared.Models;
using MediatR;
using X.PagedList;

namespace Features.GetNeighbors
{
    public class GetNeighborsQueryHandler(ServerDbContext dbContext) : VillageDataQueryHandler(dbContext), IRequestHandler<GetNeighborsQuery, IPagedList<VillageDataDto>>
    {
        public async Task<IPagedList<VillageDataDto>> Handle(GetNeighborsQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;

            var players = GetPlayers(parameters);
            var villages = GetVillages(parameters, parameters);
            var populations = GetPopulation();

            var maxDistance = Math.Pow(parameters.Distance, 2);

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
    }
}