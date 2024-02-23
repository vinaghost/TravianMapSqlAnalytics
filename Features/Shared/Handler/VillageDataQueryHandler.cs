using Core;
using Core.Entities;
using Features.Shared.Dtos;
using Features.Shared.Enums;
using Features.Shared.Models;
using Features.Shared.Parameters;
using LinqKit;
using X.PagedList;

namespace Features.Shared.Handler
{
    public abstract class VillageDataQueryHandler(ServerDbContext dbContext)
    {
        protected readonly ServerDbContext _dbContext = dbContext;

        protected static async Task<IPagedList<VillageDataDto>> ToPagedList(IEnumerable<VillageDataDto> data, IPaginationParameters parameters)
        {
            return await data.ToPagedListAsync(parameters.PageNumber, parameters.PageSize);
        }

        protected IQueryable<Player> GetPlayers(IPlayerFilterParameters parameters)
        {
            var query = _dbContext.Players
                .AsExpandable();

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

        protected IQueryable<Village> GetVillages(IVillageFilterParameters parameters, IDistanceFilterParameters distanceParameters)
        {
            var query = _dbContext.Villages
               .AsExpandable();

            if (parameters.MaxVillagePopulation != 0)
            {
                query = query
                    .Where(x => x.Population >= parameters.MinVillagePopulation)
                    .Where(x => x.Population <= parameters.MaxVillagePopulation);
            }

            if (parameters.Tribe != Tribe.All)
            {
                query = query
                    .Where(x => x.Tribe == (int)parameters.Tribe);
            }

            switch (parameters.Capital)
            {
                case Capital.Both:
                    break;

                case Capital.OnlyCapital:
                    query = query
                        .Where(x => x.IsCapital == true);
                    break;

                case Capital.OnlyVillage:
                    query = query
                        .Where(x => x.IsCapital == false);
                    break;

                default:
                    break;
            }

            if (distanceParameters.Distance != 0)
            {
                var predicate = PredicateBuilder.New<Village>();

                var center = new Coordinates(distanceParameters.X, distanceParameters.Y);

                var maxBBoxes = center.GetBoudingBoxes(distanceParameters.Distance);

                foreach (var box in maxBBoxes)
                {
                    var boxPredicate = PredicateBuilder.New<Village>()
                        .And(x => x.X >= box.MinX)
                        .And(x => x.X <= box.MaxX)
                        .And(x => x.Y >= box.MinY)
                        .And(x => x.Y <= box.MaxY);

                    predicate = predicate.Or(boxPredicate);
                }

                query = query
                    .Where(predicate);

                query = query
                    .Where(x => CoordinatesExtenstion.Distance(distanceParameters.X, distanceParameters.Y, x.X, x.Y) <= distanceParameters.Distance * distanceParameters.Distance);
            }
            return query;
        }

        protected IQueryable<VillagePopulationHistory> GetPopulation()
        {
            var query = _dbContext.VillagePopulationHistory
                .AsExpandable()
                .Where(x => x.Date >= DefaultParameters.Date);
            return query;
        }
    }
}