using Core;
using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MapSqlAspNetCoreMVC.CQRS.Queries
{
    public record GetPlayerWithDetailByInputQuery(PlayerWithDetailInput Input) : IRequest<PlayerWithDetail>;

    public class GetPlayerWithDetailWithInputQueryHandler : IRequestHandler<GetPlayerWithDetailByInputQuery, PlayerWithDetail>
    {
        private readonly ServerDbContext _context;

        public GetPlayerWithDetailWithInputQueryHandler(ServerDbContext context)
        {
            _context = context;
        }

        public async Task<PlayerWithDetail> Handle(GetPlayerWithDetailByInputQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var input = request.Input;

            var playerQuery = _context.Players
                   .Where(x => x.Name.Equals(input.PlayerName))
                   .Include(x => x.Villages)
                   .Select(x => new
                   {
                       x.PlayerId,
                       x.AllianceId,
                       PlayerName = x.Name,
                       TribeId = x.Villages.First().Tribe,
                   })
                   .Join(_context.Alliances, x => x.AllianceId, x => x.AllianceId, (player, alliance) => new
                   {
                       player.PlayerId,
                       player.PlayerName,
                       AllianceName = alliance.Name,
                       player.TribeId,
                   })
                   .FirstOrDefault();

            if (playerQuery is null)
            {
                return null;
            }

            var playerInfo = new PlayerWithDetail()
            {
                PlayerName = playerQuery.PlayerName,
                AllianceName = playerQuery.AllianceName,
                Tribe = Constants.TribeNames[playerQuery.TribeId],
                Population = [],
                AllianceNames = [],
            };

            var dates = _context.GetDateBefore(input.Days);
            var (minDate, maxDate) = (dates[^1], dates[0]);

            var populationQuery = _context.Villages
                 .Where(x => x.PlayerId == playerQuery.PlayerId)
                 .Join(_context.VillagesPopulations, x => x.VillageId, x => x.VillageId, (village, population) => new
                 {
                     village.VillageId,
                     VillageName = village.Name,
                     village.X,
                     village.Y,
                     population.Date,
                     population.Population,
                 })
                 .Where(x => x.Date >= minDate && x.Date <= maxDate)
                 .GroupBy(x => x.VillageId)
                 .AsEnumerable();

            foreach (var village in populationQuery)
            {
                var villageName = village.First().VillageName;
                var populations = new List<int>();
                foreach (var population in village)
                {
                    populations.Insert(0, population.Population);
                }
                var villageInfo = new VillageWithPopulation()
                {
                    VillageName = villageName,
                    X = village.First().X,
                    Y = village.First().Y,
                    Population = populations,
                };

                playerInfo.Population.Add(villageInfo);
            }

            var maxDays = playerInfo.Population.Max(x => x.Population.Count);

            var totalInfo = new VillageWithPopulation()
            {
                VillageName = VillageWithPopulation.Total,
                Population = new List<int>(new int[maxDays]),
            };

            foreach (var population in playerInfo.Population)
            {
                for (var i = 0; i < maxDays; i++)
                {
                    if (i >= population.Population.Count)
                    {
                        break;
                    }

                    totalInfo.Population[i] += population.Population[i];
                }
            }
            playerInfo.Population.Insert(0, totalInfo);

            var allianceQuery = _context.PlayersAlliances
                .Where(x => x.PlayerId == playerQuery.PlayerId && x.Date >= minDate && x.Date <= maxDate)
                .Join(_context.Alliances, x => x.AllianceId, x => x.AllianceId, (playerAlliance, alliance) => new
                {
                    alliance.Name,
                    playerAlliance.Date,
                })
                .OrderByDescending(x => x.Date)
                .Select(x => x.Name)
                .AsEnumerable();
            playerInfo.AllianceNames.AddRange(allianceQuery);

            return playerInfo;
        }
    }
}