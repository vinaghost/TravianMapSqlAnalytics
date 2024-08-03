using Features.Populations.Shared;
using Features.Shared.Dtos;
using Features.Shared.Query;
using Features.Villages;
using MediatR;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Features.Populations
{
    public record GetPlayersPopulationHistoryByIdQuery(PopulationParameters Parameters) : ICachedQuery<Dictionary<int, List<PopulationDto>>>
    {
        public string CacheKey => $"{nameof(GetPlayersPopulationHistoryByIdQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetPlayersPopulationHistoryByIdQueryHandler(VillageDbContext dbContext) : IRequestHandler<GetPlayersPopulationHistoryByIdQuery, Dictionary<int, List<PopulationDto>>>
    {
        protected readonly VillageDbContext _dbContext = dbContext;

        public async Task<Dictionary<int, List<PopulationDto>>> Handle(GetPlayersPopulationHistoryByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Parameters.Ids is null || request.Parameters.Ids.Count == 0)
            {
                return [];
            }

            var date = DateTime.Now.AddDays(-request.Parameters.Days);
            var population = await _dbContext.PlayersHistory
                .Where(x => request.Parameters.Ids.Contains(x.PlayerId))
                .Where(x => x.Date >= date)
                .GroupBy(x => x.PlayerId)
                .Select(x => new
                {
                    PlayerId = x.Key,
                    Population = x.Select(x => new PopulationDto(x.Date, x.Population, x.ChangePopulation)),
                })
                .ToDictionaryAsync(x => x.PlayerId, x => x.Population.ToList(), cancellationToken);

            return population;
        }
    }
}