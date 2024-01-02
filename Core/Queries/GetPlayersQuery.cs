using Core.Models;
using Core.Parameters;
using Core.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace Core.Queries
{
    public record GetPlayersQuery(PlayerParameters Parameters) : ICachedQuery<IPagedList<Player>>
    {
        public string CacheKey => $"{nameof(GetPlayersQuery)}_{Parameters.Key}";
        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetPlayersQueryHandler(UnitOfRepository unitOfRepository) : IRequestHandler<GetPlayersQuery, IPagedList<Player>>
    {
        private readonly UnitOfRepository _unitOfRepository = unitOfRepository;

        public async Task<IPagedList<Player>> Handle(GetPlayersQuery request, CancellationToken cancellationToken)
        {
            var playerQueryable = _unitOfRepository.PlayerRepository.GetQueryable(request.Parameters);
            var totalPlayer = await playerQueryable.CountAsync(cancellationToken);

            var rawPlayers = await playerQueryable
                            .Select(x => new
                            {
                                x.AllianceId,
                                x.PlayerId,
                                PlayerName = x.Name,
                                VillageCount = x.Villages.Count(),
                                Population = x.Villages.Sum(x => x.Population),
                            })
                            .OrderByDescending(x => x.VillageCount)
                            .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize, totalPlayer);
            var alliances = await _unitOfRepository.AllianceRepository.GetRecords([.. rawPlayers.Select(x => x.AllianceId)], cancellationToken);

            var players = rawPlayers
                .Select(x =>
                {
                    var alliance = alliances[x.AllianceId];
                    return new Player(
                        x.AllianceId,
                        alliance.Name,
                        x.PlayerId,
                        x.PlayerName,
                        x.VillageCount,
                        x.Population);
                });
            return players;
        }
    }
}