using Features.Shared.Dtos;
using Features.Shared.Enums;
using Features.Shared.Handler;
using Features.Shared.Models;
using Features.Shared.Parameters;
using Features.Shared.Query;
using FluentValidation;
using MediatR;
using System.Text;
using X.PagedList;

namespace Features.Villages
{
    public record GetInactiveParameters : IPaginationParameters, IPlayerFilterParameters, IVillageFilterParameters, IDistanceFilterParameters
    {
        public int InactiveDays { get; init; }

        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        public int X { get; init; }
        public int Y { get; init; }

        public int Distance { get; init; }

        public int MinPlayerPopulation { get; init; }
        public int MaxPlayerPopulation { get; init; }

        public int MinVillagePopulation { get; init; }
        public int MaxVillagePopulation { get; init; }

        public Capital Capital { get; init; }
        public Tribe Tribe { get; init; }

        public IList<int>? Alliances { get; init; }
        public IList<int>? ExcludeAlliances { get; init; }

        public IList<int>? Players { get; init; }
        public IList<int>? ExcludePlayers { get; init; }
    }

    public static class GetInactiveParametersExtension
    {
        public static string Key(this GetInactiveParameters parameters)
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '_';

            parameters.PaginationKey(sb);
            sb.Append(SEPARATOR);
            parameters.PlayerFilterKey(sb);
            sb.Append(SEPARATOR);
            parameters.VillageFilterKey(sb);
            sb.Append(SEPARATOR);
            parameters.DistanceFilterKey(sb);
            sb.Append(SEPARATOR);
            sb.Append(parameters.InactiveDays);
            sb.Append(SEPARATOR);

            return sb.ToString();
        }
    }

    public class GetInactiveParametersValidator : AbstractValidator<GetInactiveParameters>
    {
        public GetInactiveParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new DistanceFilterParametersValidator());
            Include(new PlayerFilterParametersValidator());
            Include(new VillageFilterParametersValidator());

            RuleFor(x => x.InactiveDays)
                .NotEmpty()
                .GreaterThanOrEqualTo(3);
        }
    }

    public record GetInactiveQuery(GetInactiveParameters Parameters) : ICachedQuery<IPagedList<VillageDto>>
    {
        public string CacheKey => $"{nameof(GetInactiveQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetInactiveQueryHandler(VillageDbContext dbContext) : VillageDataQueryHandler(dbContext), IRequestHandler<GetInactiveQuery, IPagedList<VillageDto>>
    {
        public async Task<IPagedList<VillageDto>> Handle(GetInactiveQuery request, CancellationToken cancellationToken)
        {
            var parameters = request.Parameters;

            var players = GetInactivePlayers(parameters);
            var villages = GetVillages(parameters, parameters);

            var data = players
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
                .Join(villages,
                    x => x.PlayerId,
                    x => x.PlayerId,
                    (player, village) => new
                    {
                        player.PlayerId,
                        player.PlayerName,
                        player.AllianceId,
                        player.AllianceName,
                        VillageId = village.Id,
                        village.MapId,
                        village.Name,
                        village.X,
                        village.Y,
                        village.Population,
                        village.Tribe,
                        village.IsCapital,
                    })
                .AsEnumerable();

            var centerCoordinate = new Coordinates(parameters.X, parameters.Y);

            var dtos = data
                .Select(x => new VillageDto(x.AllianceId,
                                            x.AllianceName,
                                            x.PlayerId,
                                            x.PlayerName,
                                            x.VillageId,
                                            x.MapId,
                                            x.Name,
                                            x.X,
                                            x.Y,
                                            x.IsCapital,
                                            (Tribe)x.Tribe,
                                            x.Population,
                                            centerCoordinate.Distance(x.X, x.Y)));

            var orderDtos = dtos
                .OrderBy(x => x.Distance);

            return await orderDtos.ToPagedListAsync(parameters.PageNumber, parameters.PageSize);
        }

        private IQueryable<int> GetInactivePlayerIds(GetInactiveParameters parameters)
        {
            var date = DateTime.Today.AddDays(-parameters.InactiveDays);
            if (IsPlayerFiltered(parameters))
            {
                var query = GetPlayers(parameters)
                    .Join(_dbContext.PlayersHistory
                            .Where(x => x.Date >= date),
                        x => x.Id,
                        x => x.PlayerId,
                        (player, population) => new
                        {
                            player.Id,
                            population.ChangePopulation
                        })
                    .GroupBy(x => x.Id)
                    .Where(x => x.Count() >= parameters.InactiveDays && x.Select(x => x.ChangePopulation).Max() == 0 && x.Select(x => x.ChangePopulation).Min() == 0)
                    .Select(x => x.Key);
                return query;
            }
            else
            {
                var query = _dbContext.PlayersHistory
                   .Where(x => x.Date >= date)
                   .GroupBy(x => x.PlayerId)
                   .Where(x => x.Count() >= parameters.InactiveDays && x.Select(x => x.ChangePopulation).Max() == 0 && x.Select(x => x.ChangePopulation).Min() == 0)
                   .Select(x => x.Key);
                return query;
            }
        }

        private IQueryable<Player> GetInactivePlayers(GetInactiveParameters parameters)
        {
            var ids = GetInactivePlayerIds(parameters);
            return _dbContext.Players
                .Where(x => ids.Contains(x.Id));
        }
    }
}