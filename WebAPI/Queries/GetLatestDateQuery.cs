using Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Queries
{
    public record GetLatestDateQuery : ICachedQuery<DateTime>
    {
        public string CacheKey => "LatestDate";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => false;
    }

    public class LatestDateQueryHandler(ServerDbContext dbContext) : IRequestHandler<GetLatestDateQuery, DateTime>
    {
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<DateTime> Handle(GetLatestDateQuery request, CancellationToken cancellationToken)
        {
            var date = await _dbContext.VillagesPopulations
                .OrderByDescending(x => x.Date)
                .Select(x => x.Date)
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            return date;
        }
    }
}