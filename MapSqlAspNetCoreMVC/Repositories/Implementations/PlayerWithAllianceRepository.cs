using MainCore;
using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using MapSqlAspNetCoreMVC.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MapSqlAspNetCoreMVC.Repositories.Implementations
{
    public class PlayerWithAllianceRepository : IPlayerWithAllianceRepository
    {
        private readonly IDbContextFactory<ServerDbContext> _contextFactory;

        public PlayerWithAllianceRepository(IDbContextFactory<ServerDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<PlayerWithAlliance>> Get(PlayerWithAllianceInput input)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var dates = context.GetDateBefore(input.Days);
            var (minDate, maxDate) = (dates[^1], dates[0]);

            var query = context.PlayersAlliances
                .Where(x => x.Date >= minDate && x.Date <= maxDate)
                .Join(context.Players, x => x.PlayerId, x => x.PlayerId, (alliances, players) => new
                {
                    players.PlayerId,
                    players.Name,
                    alliances.AllianceId,
                    alliances.Date,
                })
                .Join(context.Alliances, x => x.AllianceId, x => x.AllianceId, (player, alliance) => new
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

            var players = query.Select(x => new PlayerWithAlliance()
            {
                PlayerId = x.PlayerId,
                PlayerName = x.PlayerName,
                AllianceChangeNumber = x.Alliance.Distinct().Count() - 1,
                AllianceNames = x.Alliance,
            }).ToList();
            return players;
        }
    }
}