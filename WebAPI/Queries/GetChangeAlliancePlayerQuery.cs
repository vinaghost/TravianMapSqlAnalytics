using MediatR;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using WebAPI.Repositories;
using X.PagedList;

namespace WebAPI.Queries
{
    public record GetChangeAlliancePlayersQuery(ChangeAlliancePlayerParameters Parameters) : ICachedQuery<IPagedList<ChangeAlliancePlayer>>
    {
        public string CacheKey => Parameters.Key;
        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetChangeAlliancePlayersQueryHandler(UnitOfRepository unitOfRepository) : IRequestHandler<GetChangeAlliancePlayersQuery, IPagedList<ChangeAlliancePlayer>>
    {
        private readonly UnitOfRepository _unitOfRepository = unitOfRepository;

        public async Task<IPagedList<ChangeAlliancePlayer>> Handle(GetChangeAlliancePlayersQuery request, CancellationToken cancellationToken)
        {
            var date = request.Parameters.Date.ToDateTime(TimeOnly.MinValue);
            var playerQueryable = _unitOfRepository.PlayerRepository.GetQueryable(request.Parameters);

            var rawPlayers = await playerQueryable
                .Select(x => new
                {
                    x.AllianceId,
                    x.PlayerId,
                    PlayerName = x.Name,
                    Alliances = x.Alliances
                        .Where(x => x.Date >= date)
                        .Select(x => new
                        {
                            x.Date,
                            x.AllianceId,
                        })
                })
                .AsEnumerable()
                .Select(x => new
                {
                    x.AllianceId,
                    x.PlayerId,
                    x.PlayerName,
                    ChangeAlliance = x.Alliances.DistinctBy(x => x.AllianceId).Count(),
                    x.Alliances
                })
                .Where(x => x.ChangeAlliance >= request.Parameters.MinChangeAlliance)
                .Where(x => x.ChangeAlliance <= request.Parameters.MaxChangeAlliance)
                .OrderByDescending(x => x.ChangeAlliance)
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);

            var oldAllianceId = rawPlayers.SelectMany(x => x.Alliances).DistinctBy(x => x.AllianceId).Select(x => x.AllianceId);
            var currentAllianceId = rawPlayers.Select(x => x.AllianceId);
            var alliances = await _unitOfRepository.AllianceRepository.GetRecords([.. currentAllianceId.Concat(oldAllianceId)], cancellationToken);

            var players = rawPlayers
                .Select(x =>
                {
                    var alliance = alliances[x.AllianceId];

                    return new ChangeAlliancePlayer(
                            x.AllianceId,
                            alliance.Name,
                            x.PlayerId,
                            x.PlayerName,
                            x.ChangeAlliance,
                            x.Alliances.Select(ally => new AllianceHistoryRecord(
                                ally.AllianceId,
                                alliance.Name,
                                ally.Date)));
                });
            return players;
        }
    }
}