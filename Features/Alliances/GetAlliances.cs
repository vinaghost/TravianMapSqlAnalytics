using Features.Shared.Dtos;
using Features.Shared.Query;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Features.Alliances
{
    public record GetAlliancesParameters(IList<int> Ids);

    public static class GetAlliancesParametersExtension
    {
        public static string Key(this GetAlliancesParameters parameters)
        {
            return string.Join(',', parameters.Ids.Distinct().Order());
        }
    }

    public class GetAlliancesParametersValidator : AbstractValidator<GetAlliancesParameters>
    {
        public GetAlliancesParametersValidator()
        {
            RuleFor(x => x.Ids).NotEmpty();
        }
    }

    public record GetAlliancesQuery(GetAlliancesParameters Parameters) : ICachedQuery<IList<AllianceDto>>
    {
        public string CacheKey => $"{nameof(GetAlliancesQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetAlliancesQueryHandler(VillageDbContext dbContext) : IRequestHandler<GetAlliancesQuery, IList<AllianceDto>>
    {
        protected readonly VillageDbContext _dbContext = dbContext;

        public async Task<IList<AllianceDto>> Handle(GetAlliancesQuery request, CancellationToken cancellationToken)
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