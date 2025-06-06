using Features.Dtos;
using Features.Parameters;
using Features.Queries.Villages;
using Features.Shared.Constraints;
using FluentValidation;
using Immediate.Handlers.Shared;
using Infrastructure.DbContexts;
using LinqKit;
using System.Text;
using X.PagedList;
using X.PagedList.Extensions;

namespace Features.Queries.Players
{
    public record GetPlayersParameters : IPaginationParameters, IPlayerFilterParameters
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

    public static class GetPlayersParametersExtension
    {
        public static string Key(this GetPlayersParameters parameters)
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '_';

            parameters.PaginationKey(sb);
            sb.Append(SEPARATOR);
            parameters.PlayerFilterKey(sb);

            return sb.ToString();
        }
    }

    public class GetPlayersParametersValidator : AbstractValidator<GetPlayersParameters>
    {
        public GetPlayersParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new PlayerFilterParametersValidator());
        }
    }

    [Handler]
    public static partial class GetPlayersQuery
    {
        public sealed record Query(GetPlayersParameters Parameters)
            : DefaultCachedQuery($"{nameof(GetPlayersQuery)}_{Parameters.Key()}");

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