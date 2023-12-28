using Core;
using MediatR;
using PlayerEnitty = Core.Models.Player;

namespace WebAPI.Queries
{
    public record FilteredPlayerQuery(List<int> Alliances) : IRequest<IQueryable<PlayerEnitty>>;

    public class PlayerQueryHandler(ServerDbContext dbContext) : IRequestHandler<FilteredPlayerQuery, IQueryable<PlayerEnitty>>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<IQueryable<PlayerEnitty>> Handle(FilteredPlayerQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var filterQuery = GetQuery(request);
            return filterQuery;
        }

        private IQueryable<PlayerEnitty> GetQuery(FilteredPlayerQuery request)
        {
            if (request.Alliances.Count > 0)
            {
                return _dbContext.Alliances
                     .Where(x => request.Alliances.Contains(x.AllianceId))
                     .SelectMany(x => x.Players);
            }
            else
            {
                return _dbContext.Players.AsQueryable();
            }
        }
    }
}