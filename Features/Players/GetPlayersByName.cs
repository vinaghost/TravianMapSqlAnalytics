using Features.Shared.Dtos;
using Features.Shared.Parameters;
using Features.Shared.Query;
using FluentValidation;
using MediatR;
using System.Text;
using X.PagedList;

namespace Features.Players
{
    public record GetPlayersByNameParameters : IPaginationParameters, ISearchTermParameters
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public string? SearchTerm { get; init; }
    }

    public static class GetPlayersByNameParametersExtension
    {
        public static string Key(this GetPlayersByNameParameters parameters)
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '_';

            parameters.PaginationKey(sb);
            sb.Append(SEPARATOR);
            parameters.SearchTermKey(sb);

            return sb.ToString();
        }
    }

    public class GetPlayersByNameParametersValidator : AbstractValidator<GetPlayersByNameParameters>
    {
        public GetPlayersByNameParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new SearchTermParametersValidator());
        }
    }

    public record GetPlayersByNameQuery(GetPlayersByNameParameters Parameters) : ICachedQuery<IPagedList<PlayerDto>>
    {
        public string CacheKey => $"{nameof(GetPlayersByNameQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetPlayersByNameQueryHandler(VillageDbContext dbContext) : IRequestHandler<GetPlayersByNameQuery, IPagedList<PlayerDto>>
    {
        private readonly VillageDbContext _dbContext = dbContext;

        public async Task<IPagedList<PlayerDto>> Handle(GetPlayersByNameQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;
            var query = _dbContext.Players
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                query = query
                    .Where(x => x.Name.StartsWith(parameters.SearchTerm));
            }

            var data = await query
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
                .OrderBy(x => x.PlayerName)
                .Select(x => new PlayerDto(
                        x.AllianceId,
                        x.AllianceName,
                        x.PlayerId,
                        x.PlayerName,
                        x.VillageCount,
                        x.Population
                ))
                .ToPagedListAsync(parameters.PageNumber, parameters.PageSize);

            return data;
        }
    }
}