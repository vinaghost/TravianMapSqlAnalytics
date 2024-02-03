using ConsoleUpdate.Models;
using Core.Entities;

namespace ConsoleUpdate.Extensions
{
    public static class VillageRawExtension
    {
        public static Alliance GetAlliace(this VillageRaw villageRaw)
        {
            return new Alliance
            {
                Id = villageRaw.AllianceId,
                Name = villageRaw.AllianceName
            };
        }

        public static Player GetPlayer(this VillageRaw villageRaw)
        {
            return new Player
            {
                Id = villageRaw.PlayerId,
                Name = villageRaw.PlayerName,
                AllianceId = villageRaw.AllianceId
            };
        }

        public static Village GetVillage(this VillageRaw villageRaw)
        {
            return new Village
            {
                Id = villageRaw.VillageId,
                MapId = villageRaw.MapId,
                Name = villageRaw.VillageName,
                Tribe = villageRaw.Tribe,
                X = villageRaw.X,
                Y = villageRaw.Y,
                PlayerId = villageRaw.PlayerId,
                IsCapital = villageRaw.IsCapital,
                IsCity = villageRaw.IsCity,
                IsHarbor = villageRaw.IsHarbor,
                Population = villageRaw.Population,
                Region = villageRaw.Region,
                VictoryPoints = villageRaw.VictoryPoints
            };
        }

        public static VillagePopulationHistory GetVillagePopulation(this Village village, DateTime date)
        {
            return new VillagePopulationHistory
            {
                VillageId = village.Id,
                Population = village.Population,
                Date = date,
            };
        }

        public static PlayerPopulationHistory GetPlayerPopulation(this Player player, DateTime date)
        {
            return new PlayerPopulationHistory
            {
                PlayerId = player.Id,
                Population = player.Population,
                Date = date,
            };
        }

        public static PlayerAllianceHistory GetPlayerAlliance(this Player player, DateTime date)
        {
            return new PlayerAllianceHistory
            {
                PlayerId = player.Id,
                AllianceId = player.AllianceId,
                Date = date,
            };
        }
    }
}