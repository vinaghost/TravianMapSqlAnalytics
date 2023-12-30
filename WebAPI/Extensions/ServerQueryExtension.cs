using Core;
using Core.Models;
using WebAPI.Models.Parameters;

namespace WebAPI.Extensions
{
    public static class ServerQueryExtension
    {
        public static IQueryable<Village> GetQueryable(this ServerDbContext dbContext, IVillageFilterParameter parameter)
        {
            return dbContext.GetVillageQueryable(parameter)
                 .Where(x => x.Population >= parameter.MinPopulation)
                 .Where(x => x.Population <= parameter.MaxPopulation);
        }

        private static IQueryable<Village> GetVillageQueryable(this ServerDbContext dbContext, IVillageFilterParameter parameter)
        {
            if (parameter.Alliances.Count > 0)
            {
                return dbContext.Alliances
                     .Where(x => parameter.Alliances.Contains(x.AllianceId))
                     .SelectMany(x => x.Players)
                     .SelectMany(x => x.Villages);
            }
            else if (parameter.Players.Count > 0)
            {
                return dbContext.Players
                     .Where(x => parameter.Players.Contains(x.PlayerId))
                     .SelectMany(x => x.Villages);
            }
            else if (parameter.Villages.Count > 0)
            {
                return dbContext.Villages
                    .Where(x => parameter.Villages.Contains(x.PlayerId));
            }
            else
            {
                return dbContext.Villages.AsQueryable();
            }
        }

        public static IQueryable<Player> GetQueryable(this ServerDbContext dbContext, IPlayerFilterParameter parameter)
        {
            if (parameter.Alliances.Count > 0)
            {
                return dbContext.Alliances
                     .Where(x => parameter.Alliances.Contains(x.AllianceId))
                     .SelectMany(x => x.Players);
            }
            else if (parameter.Players.Count > 0)
            {
                return dbContext.Players
                     .Where(x => parameter.Players.Contains(x.PlayerId));
            }
            else
            {
                return dbContext.Players.AsQueryable();
            }
        }
    }
}