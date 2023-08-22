using MainCore;
using MapSqlAspNetCoreMVC.Models;
using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using MapSqlAspNetCoreMVC.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MapSqlAspNetCoreMVC.Repositories.Implementations
{
    public class VillageRepository : IVillageRepository
    {
        private readonly IDbContextFactory<ServerDbContext> _contextFactory;

        public VillageRepository(IDbContextFactory<ServerDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Village>> Get(VillageFilterFormInput input)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var query = context.Villages
                .Where(x => x.Population >= input.MinPop);
            if (input.MaxPop != -1 && input.MaxPop > input.MinPop)
            {
                query = query.Where(x => x.Population <= input.MaxPop);
            }
            if (input.TribeId != 0)
            {
                query = query.Where(x => x.Tribe == input.TribeId);
            }

            var joinQuery = query.Join(context.Players, x => x.PlayerId, x => x.PlayerId, (village, player) => new
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
            })
            .Join(context.Alliances, x => x.AllianceId, x => x.AllianceId, (village, alliance) => new
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
                joinQuery = joinQuery.Where(x => x.AllianceId == input.AllianceId);
            }

            var result = joinQuery.AsEnumerable();

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