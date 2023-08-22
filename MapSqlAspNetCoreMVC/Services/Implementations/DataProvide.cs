﻿using MainCore;
using MapSqlAspNetCoreMVC.Models.Input;
using MapSqlAspNetCoreMVC.Models.Output;
using MapSqlAspNetCoreMVC.Repositories.Interfaces;
using MapSqlAspNetCoreMVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace MapSqlAspNetCoreMVC.Services.Implementations
{
    public class DataProvide : IDataProvide
    {
        private readonly IDbContextFactory<ServerDbContext> _contextFactory;

        private readonly IPlayerWithPopulationRepository _playerWithPopulationRepository;
        private readonly IPlayerWithVillagePopulationRepository _playerWithVillagePopulationRepository;
        private readonly IVillageRepository _villageRepository;

        private readonly List<SelectListItem> _tribeNamesList = new()
        {
            new SelectListItem {Value = "0", Text = "All"},
            new SelectListItem {Value = "1", Text = "Romans"},
            new SelectListItem {Value = "2", Text = "Teutons"},
            new SelectListItem {Value = "3", Text = "Gauls"},
            new SelectListItem {Value = "4", Text = "Nature"},
            new SelectListItem {Value = "5", Text = "Natars"},
            new SelectListItem {Value = "6", Text = "Egyptians"},
            new SelectListItem {Value = "7", Text = "Huns"},
            new SelectListItem {Value = "8", Text = "Spartans"},
        };

        public DataProvide(IDbContextFactory<ServerDbContext> contextFactory, IPlayerWithPopulationRepository playerWithPopulationRepository, IPlayerWithVillagePopulationRepository playerWithVillagePopulationRepository, IVillageRepository villageRepository)
        {
            _contextFactory = contextFactory;
            _playerWithPopulationRepository = playerWithPopulationRepository;
            _playerWithVillagePopulationRepository = playerWithVillagePopulationRepository;
            _villageRepository = villageRepository;
        }

        public async Task<List<PlayerWithPopulation>> GetInactivePlayerData(InactiveFormInput input)
        {
            var players = await _playerWithPopulationRepository.Get(input);
            var filterdPlayers = players
                .Where(x => x.Population.Count > input.Days && x.Population.Max() - x.Population.Min() == 0)
                .ToList();
            return filterdPlayers;
        }

        public async Task<List<Village>> GetVillageData(VillageFilterFormInput input)
        {
            var villages = await _villageRepository.Get(input);
            return villages;
        }

        public async Task<PlayerWithVillagePopulation> GetPlayerInfo(PlayerLookupInput input)
        {
            var player = await _playerWithVillagePopulationRepository.Get(input);
            return player;
        }

        public List<DateTime> GetDateBefore(int days)
        {
            using var context = _contextFactory.CreateDbContext();
            return context.GetDateBefore(days);
        }

        public DateTime GetNewestDay()
        {
            using var context = _contextFactory.CreateDbContext();
            return context.GetNewestDay();
        }

        public List<SelectListItem> GetAllianceSelectList()
        {
            using var context = _contextFactory.CreateDbContext();

            var alliances = new List<SelectListItem>
            {
                new SelectListItem { Value = "-1", Text = "All" }
            };

            var query = context.Alliances
                .Include(x => x.Players)
                .OrderByDescending(x => x.Players.Count)
                .Select(x => new SelectListItem
                {
                    Value = $"{x.AllianceId}",
                    Text = x.Name,
                })
                .AsEnumerable();

            alliances.AddRange(query);
            return alliances;
        }

        public List<SelectListItem> GetTribeSelectList()
        {
            return _tribeNamesList;
        }
    }
}