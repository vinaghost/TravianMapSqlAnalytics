using Features.Shared.Handler;
using Features.Shared.Parameters;
using Features.Shared.Query;
using MediatR;
using System.Text;
using X.PagedList;

namespace Features.Players
{
    public record PlayerParameters : IPaginationParameters, IPlayerFilterParameters
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public int MinPlayerPopulation { get; init; }
        public int MaxPlayerPopulation { get; init; }

        public IList<int>? Alliances { get; init; }
        public IList<int>? ExcludeAlliances { get; init; }
    }

    public static class PlayerParametersExtension
    {
        public static string Key(this PlayerParameters parameters)
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '_';

            sb.Append(parameters.PageNumber);
            sb.Append(SEPARATOR);
            sb.Append(parameters.PageSize);
            sb.Append(SEPARATOR);
            sb.Append(parameters.MinPlayerPopulation);
            sb.Append(SEPARATOR);
            sb.Append(parameters.MaxPlayerPopulation);

            if (parameters.Alliances is not null && parameters.Alliances.Count > 0)
            {
                sb.Append(SEPARATOR);
                sb.AppendJoin(',', parameters.Alliances.Distinct().Order());
            }
            else if (parameters.ExcludeAlliances is not null && parameters.ExcludeAlliances.Count > 0)
            {
                sb.Append(SEPARATOR);
                sb.Append(SEPARATOR);
                sb.AppendJoin(',', parameters.ExcludeAlliances.Distinct().Order());
            }

            return sb.ToString();
        }
    }

    public record GetAllPlayerQuery(PlayerParameters Parameters) : ICachedQuery<IPagedList<PlayerDto>>
    {
        public string CacheKey => $"{nameof(GetPlayerQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetAllPlayerQueryHandler(VillageDbContext dbContext) : VillageDataQueryHandler(dbContext), IRequestHandler<GetAllPlayerQuery, IPagedList<PlayerDto>>
    {
        public async Task<IPagedList<PlayerDto>> Handle(GetAllPlayerQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;
            var players = await GetPlayers(parameters)
                .OrderByDescending(x => x.VillageCount)
                .Select(x => new PlayerDto(
                        x.Id,
                        x.Name,
                        x.VillageCount,
                        x.Population
                    ))
                .ToPagedListAsync(parameters.PageNumber, parameters.PageSize);
            return players;
        }
    }
}