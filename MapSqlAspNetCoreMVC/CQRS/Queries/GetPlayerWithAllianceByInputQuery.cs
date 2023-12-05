using Core;
using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using MediatR;

namespace MapSqlAspNetCoreMVC.CQRS.Queries
{
    public record GetPlayerWithAllianceByInputQuery(PlayerWithAllianceInput Input) : IRequest<List<PlayerWithAlliance>>;

    public class GetPlayerWithAllianceByInputQueryHandler : IRequestHandler<GetPlayerWithAllianceByInputQuery, List<PlayerWithAlliance>>
    {
        private readonly ServerDbContext _context;

        public GetPlayerWithAllianceByInputQueryHandler(ServerDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlayerWithAlliance>> Handle(GetPlayerWithAllianceByInputQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var input = request.Input;

            var dates = _context.GetDateBefore(input.Days);
            var (minDate, maxDate) = (dates[^1], dates[0]);

            var query = _context.PlayersAlliances
                .Where(x => x.Date >= minDate && x.Date <= maxDate)
                .Join(_context.Players, x => x.PlayerId, x => x.PlayerId, (alliances, players) => new
                {
                    players.PlayerId,
                    players.Name,
                    alliances.AllianceId,
                    alliances.Date,
                })
                .Join(_context.Alliances, x => x.AllianceId, x => x.AllianceId, (player, alliance) => new
                {
                    player.PlayerId,
                    PlayerName = player.Name,
                    AllianceName = alliance.Name,
                    player.Date,
                })
                .GroupBy(x => x.PlayerId)
                .Select(x => new
                {
                    PlayerId = x.Key,
                    PlayerName = x.Select(x => x.PlayerName).FirstOrDefault(),
                    Alliance = x.OrderByDescending(x => x.Date).Select(x => x.AllianceName).ToList(),
                })
                .AsEnumerable();
            var players = query
                .Select(x => new PlayerWithAlliance()
                {
                    PlayerId = x.PlayerId,
                    PlayerName = x.PlayerName,
                    AllianceChangeNumber = x.Alliance.Distinct().Count() - 1,
                    AllianceNames = x.Alliance,
                })
                .Where(x => x.AllianceChangeNumber != 0)
                .ToList();
            return players;
        }
    }
}