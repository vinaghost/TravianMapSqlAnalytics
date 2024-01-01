﻿using MediatR;
using WebAPI.Models.Output;
using WebAPI.Models.Parameters;
using WebAPI.Repositories;
using X.PagedList;

namespace WebAPI.Queries
{
    public record GetChangePopulationPlayersQuery(ChangePopulationPlayerParameters Parameters) : IQuery<IPagedList<ChangePopulationPlayer>>;

    public class GetChangePopulationPlayersQueryHandler(UnitOfRepository unitOfRepository) : IRequestHandler<GetChangePopulationPlayersQuery, IPagedList<ChangePopulationPlayer>>
    {
        private readonly UnitOfRepository _unitOfRepository = unitOfRepository;

        public async Task<IPagedList<ChangePopulationPlayer>> Handle(GetChangePopulationPlayersQuery request, CancellationToken cancellationToken)
        {
            var date = request.Parameters.Date.ToDateTime(TimeOnly.MinValue);
            var playerQueryable = _unitOfRepository.PlayerRepository.GetQueryable(request.Parameters);

            var rawPlayers = await playerQueryable
                .Select(x => new
                {
                    x.AllianceId,
                    x.PlayerId,
                    PlayerName = x.Name,
                    Populations = x.Villages
                        .SelectMany(x => x.Populations
                                        .Where(x => x.Date >= date))
                        .GroupBy(x => x.Date)
                        .OrderByDescending(x => x.Key)
                        .Select(x => new
                        {
                            Date = x.Key,
                            Population = x
                                    .OrderBy(x => x.Date)
                                    .Select(x => x.Population)
                                    .Sum(),
                        })
                })
                .AsEnumerable()
                .Select(x => new
                {
                    x.AllianceId,
                    x.PlayerId,
                    x.PlayerName,
                    ChangePopulation = x.Populations.Select(x => x.Population).FirstOrDefault() - x.Populations.Select(x => x.Population).LastOrDefault(),
                    Populations = x.Populations.Select(x => new RecordPopulation(x.Population, x.Date))
                })
                .Where(x => x.ChangePopulation >= request.Parameters.MinChangePopulation)
                .Where(x => x.ChangePopulation <= request.Parameters.MaxChangePopulation)
                .OrderByDescending(x => x.ChangePopulation)
                .ToPagedListAsync(request.Parameters.PageNumber, request.Parameters.PageSize);

            var alliances = await _unitOfRepository.AllianceRepository.GetRecords([.. rawPlayers.Select(x => x.AllianceId)], cancellationToken);

            var players = rawPlayers
               .Select(x =>
               {
                   var alliance = alliances[x.AllianceId];
                   return new ChangePopulationPlayer(
                       x.AllianceId,
                       alliance.Name,
                       x.PlayerId,
                       x.PlayerName,
                       x.ChangePopulation,
                       x.Populations);
               });
            return players;
        }
    }
}