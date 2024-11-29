using Features.Shared.Dtos;
using Features.Shared.Handler;
using Features.Shared.Parameters;
using Features.Shared.Query;
using System.Text;

namespace Features.Players
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

    public record GetPlayersByParametersQuery(PlayersParameters Parameters) : ICachedQuery<IPagedList<PlayerDto>>
    {
        public string CacheKey => $"{nameof(GetPlayersByParametersQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetPlayersByParametersQueryHandler(VillageDbContext dbContext) : IRequestHandler<GetPlayersByParametersQuery, IPagedList<PlayerDto>>
    {
        private readonly VillageDbContext _dbContext = dbContext;

        public async Task<IPagedList<PlayerDto>> Handle(GetPlayersByParametersQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;
            var predicate = VillageDataQuery.PlayerPredicate(parameters);
            var players = _dbContext.Players
                .AsExpandable()
                .Where(predicate)
                .OrderByDescending(x => x.VillageCount)
                .Join(_dbContext.Alliances,
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
            return players;
        }
    }
}