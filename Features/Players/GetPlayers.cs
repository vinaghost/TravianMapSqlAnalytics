using Features.Shared.Dtos;
using Features.Shared.Handler;
using Features.Shared.Parameters;
using Features.Shared.Query;
using FluentValidation;
using MediatR;
using System.Text;
using X.PagedList;

namespace Features.Players
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

    public record GetPlayersQuery(GetPlayersParameters Parameters) : ICachedQuery<IPagedList<PlayerDto>>
    {
        public string CacheKey => $"{nameof(GetPlayersQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetPlayersQueryHandler(VillageDbContext dbContext) : VillageDataQueryHandler(dbContext), IRequestHandler<GetPlayersQuery, IPagedList<PlayerDto>>
    {
        public async Task<IPagedList<PlayerDto>> Handle(GetPlayersQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;
            var players = await GetPlayers(parameters)
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
                .OrderByDescending(x => x.VillageCount)
                .Select(x => new PlayerDto(
                        x.AllianceId,
                        x.AllianceName,
                        x.PlayerId,
                        x.PlayerName,
                        x.VillageCount,
                        x.Population
                ))
                .ToPagedListAsync(parameters.PageNumber, parameters.PageSize);
            return players;
        }
    }
}