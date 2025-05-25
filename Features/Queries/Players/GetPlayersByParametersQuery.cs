using Features.Constraints;
using Features.Shared.Dtos;
using Features.Shared.Handler;
using Features.Shared.Parameters;
using Immediate.Handlers.Shared;
using System.Text;

namespace Features.Queries.Players
{
    public record PlayersParameters : IPaginationParameters, IPlayerFilterParameters
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public int MinPlayerPopulation { get; init; }
        public int MaxPlayerPopulation { get; init; }

        public IList<int>? Alliances { get; init; }
        public IList<int>? ExcludeAlliances { get; init; }

        public IList<int>? Players { get; init; }
        public IList<int>? ExcludePlayers { get; init; }
    }

    public static class PlayersParametersExtension
    {
        public static string Key(this PlayersParameters parameters)
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '_';

            parameters.PaginationKey(sb);
            sb.Append(SEPARATOR);
            parameters.PlayerFilterKey(sb);

            return sb.ToString();
        }
    }

    public class PlayersParametersValidator : AbstractValidator<PlayersParameters>
    {
        public PlayersParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new PlayerFilterParametersValidator());
        }
    }

    [Handler]
    public static partial class GetPlayersByParametersQuery
    {
        public sealed record Query(PlayersParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetPlayersByParametersQuery)}_{Parameters.Key()}");

        private static ValueTask<IPagedList<PlayerDto>> HandleAsync(
            Query query,
            VillageDbContext context,
            CancellationToken cancellationToken
        )
        {
            var parameters = query.Parameters;
            var predicate = VillageDataQuery.PlayerPredicate(parameters);
            var players = context.Players
                .AsExpandable()
                .Where(predicate)
                .OrderByDescending(x => x.VillageCount)
                .Join(context.Alliances,
                   x => x.AllianceId,
                   x => x.Id,
                   (player, alliance) => new PlayerDto
                   (
                       alliance.Id,
                       alliance.Name,
                       player.Id,
                       player.Name,
                       player.VillageCount,
                       player.Population
                   ))
                .ToPagedList(parameters.PageNumber, parameters.PageSize);
            return ValueTask.FromResult(players);
        }
    }
}