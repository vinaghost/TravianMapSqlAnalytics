using Core.Dtos;
using Core.Parameters;
using Core.Repositories;
using MediatR;
using X.PagedList;

namespace Core.Queries
{
    public record GetPlayerContainsPopulationQuery(PlayerContainsPopulationParameters Parameters) : ICachedQuery<IPagedList<PlayerContainsPopulationDto>>
    {
        public string CacheKey => $"{nameof(GetPlayerContainsPopulationQuery)}_{Parameters.Key}";
        public TimeSpan? Expiation => null;
        public bool IsServerBased => true;
    }

    public class GetPlayerContainsPopulationQueryHandler(ServerDbContext dbContext, UnitOfRepository unitOfRepository) : IRequestHandler<GetPlayerContainsPopulationQuery, IPagedList<PlayerContainsPopulationDto>>
    {
        private readonly UnitOfRepository _unitOfRepository = unitOfRepository;
        private readonly ServerDbContext _dbContext = dbContext;

        public async Task<IPagedList<PlayerContainsPopulationDto>> Handle(GetPlayerContainsPopulationQuery request, CancellationToken cancellationToken)
        {
            var playerIds = await _unitOfRepository.PlayerRepository.GetPlayerIds(request.Parameters, cancellationToken);
            var players = await _unitOfRepository.PlayerRepository.GetPlayerInfo(playerIds, cancellationToken);
            var alliances = await _unitOfRepository.AllianceRepository.GetRecords([.. players.Keys], cancellationToken);

            return await _unitOfRepository.PlayerRepository.GetPlayers([.. players.Keys])
                .Select(x =>
                {
                    var alliance = alliances[x.AllianceId];
                    var player = players[x.PlayerId];
                    return new PlayerContainsPopulationDto(
                        x.AllianceId,
                        alliance.Name,
                        x.PlayerId,
                        x.PlayerName,
                        player.VillageCount,
                        player.Population);
                })
                .OrderByDescending(x => x.VillageCount)
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);
        }
    }
}