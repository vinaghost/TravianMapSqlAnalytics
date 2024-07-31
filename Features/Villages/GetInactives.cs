using Features.Shared.Enums;
using Features.Shared.Handler;
using Features.Shared.Models;
using Features.Shared.Parameters;
using Features.Shared.Query;
using Features.Shared.Validators;
using FluentValidation;
using MediatR;
using System.Text;
using X.PagedList;

namespace Features.Villages
{
    public record InactiveDto(int PlayerId,
                              string PlayerName,
                              int AllianceId,
                              string AllianceName,
                              int MapId,
                              string VillageName,
                              int X,
                              int Y,
                              bool IsCapital,
                              Tribe Tribe,
                              int Population,
                              double Distance);

    public record InactiveParameters : IPaginationParameters, IPlayerFilterParameters, IVillageFilterParameters, IDistanceFilterParameters
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
    }

    public static class InactiveFarmParametersExtension
    {
        public static string Key(this InactiveParameters parameters)
        {
            var sb = new StringBuilder();
            const char SEPARATOR = '_';

            sb.Append(parameters.PageNumber);
            sb.Append(SEPARATOR);
            sb.Append(parameters.PageSize);
            sb.Append(SEPARATOR);
            sb.Append(parameters.X);
            sb.Append(SEPARATOR);
            sb.Append(parameters.Y);
            sb.Append(SEPARATOR);
            sb.Append(parameters.Distance);
            sb.Append(SEPARATOR);
            sb.Append(parameters.InactiveDays);
            sb.Append(SEPARATOR);
            sb.Append(parameters.MinPlayerPopulation);
            sb.Append(SEPARATOR);
            sb.Append(parameters.MaxPlayerPopulation);
            sb.Append(SEPARATOR);
            sb.Append(parameters.MinVillagePopulation);
            sb.Append(SEPARATOR);
            sb.Append(parameters.MaxVillagePopulation);

            sb.Append(SEPARATOR);
            sb.Append(parameters.Capital);
            sb.Append(SEPARATOR);
            sb.Append(parameters.Tribe);

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

    public class InactiveParametersValidator : AbstractValidator<InactiveParameters>
    {
        public InactiveParametersValidator()
        {
            Include(new PaginationParametersValidator());
            Include(new DistanceFilterParametersValidator());
            Include(new PlayerPopulationFilterParametersValidator());
            Include(new VillagePopulationFilterParametersValidator());

            RuleFor(x => x.InactiveDays)
                .NotEmpty()
                .GreaterThanOrEqualTo(3);
        }
    }

    public record GetInactiveQuery(InactiveParameters Parameters) : ICachedQuery<IPagedList<InactiveDto>>
    {
        public string CacheKey => $"{nameof(GetInactiveQuery)}_{Parameters.Key()}";

        public TimeSpan? Expiation => null;

        public bool IsServerBased => true;
    }

    public class GetInactiveQueryHandler(VillageDbContext dbContext) : VillageDataQueryHandler(dbContext), IRequestHandler<GetInactiveQuery, IPagedList<InactiveDto>>
    {
        public async Task<IPagedList<InactiveDto>> Handle(GetInactiveQuery request, CancellationToken cancellationToken)
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
                .Select(x => new InactiveDto(x.PlayerId,
                                             x.PlayerName,
                                             x.AllianceId,
                                             x.AllianceName,
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

        private static bool IsPlayerFiltered(IPlayerFilterParameters parameters)
        {
            if (parameters.Alliances is not null && parameters.Alliances.Count > 0) return true;
            if (parameters.ExcludeAlliances is not null && parameters.ExcludeAlliances.Count > 0) return true;
            if (parameters.MaxPlayerPopulation != 0) return true;
            return false;
        }

        private IQueryable<int> GetInactivePlayerIds(InactiveParameters parameters)
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

        private IQueryable<Player> GetInactivePlayers(InactiveParameters parameters)
        {
            var ids = GetInactivePlayerIds(parameters);
            return _dbContext.Players
                .Where(x => ids.Contains(x.Id));
        }
    }
}