using Features.Constraints;
using Features.Dtos;
using Features.Parameters;
using Immediate.Handlers.Shared;
using System.Text;

namespace Features.Queries.Players
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

    [Handler]
    public static partial class GetPlayersByNameQuery
    {
        public sealed record Query(GetPlayersByNameParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetPlayersByNameQuery)}_{Parameters.Key()}");

        private static ValueTask<IPagedList<PlayerDto>> HandleAsync(
            Query query,
            VillageDbContext context,
            CancellationToken cancellationToken
        )
        {
            var parameters = query.Parameters;
            var dbQuery = context.Players
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                dbQuery = dbQuery
                    .Where(x => x.Name.StartsWith(parameters.SearchTerm));
            }

            var data = dbQuery
               .Join(context.Alliances,
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
                .ToPagedList(parameters.PageNumber, parameters.PageSize);

            return ValueTask.FromResult(data);
        }
    }
}