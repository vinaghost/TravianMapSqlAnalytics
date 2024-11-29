using Features.Shared.Dtos;
using Features.Shared.Query;
using Microsoft.EntityFrameworkCore;

namespace Features.Alliances
{
    public record GetAlliancesByIdParameters(IList<int> Ids);

    public static class GetAlliancesByIdParametersExtension
    {
        public static string Key(this GetAlliancesByIdParameters parameters)
        {
            return string.Join(',', parameters.Ids.Distinct().Order());
        }
    }

    public class GetAlliancesByIdParametersValidator : AbstractValidator<GetAlliancesByIdParameters>
    {
        public GetAlliancesByIdParametersValidator()
        {
            RuleFor(x => x.Ids).NotEmpty();
        }
    }

    public record GetAlliancesByIdQuery(GetAlliancesByIdParameters Parameters) : ICachedQuery<IList<AllianceDto>>
    {
        public string CacheKey => $"{nameof(GetAlliancesByIdQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetAlliancesByIdQueryHandler(VillageDbContext dbContext) : IRequestHandler<GetAlliancesByIdQuery, IList<AllianceDto>>
    {
        protected readonly VillageDbContext _dbContext = dbContext;

        public async Task<IList<AllianceDto>> Handle(GetAlliancesByIdQuery request, CancellationToken cancellationToken)
        {
            var ids = request.Parameters.Ids.Distinct().ToList();
            if (ids.Count == 0) return [];

            var query = _dbContext.Alliances
                .AsQueryable();

            if (ids.Count == 1)
            {
                query
                    .Where(x => x.Id == ids[0]);
            }
            else
            {
                query
                    .Where(x => ids.Contains(x.Id));
            }

            var alliances = await query
                            .Select(x => new AllianceDto(
                                x.Id,
                                x.Name,
                                x.PlayerCount
                                ))
                            .ToListAsync();
            return alliances;
        }
    }
}