using MainCore.Models;
using MapSqlDatabaseUpdate.Models;

namespace MapSqlDatabaseUpdate.Extensions
{
    public static class VillageRawExtension
    {
        public static Alliance GetAlliace(this VillageRaw villageRaw)
        {
            return new Alliance
            {
                AllianceId = villageRaw.AllianceId,
                Name = villageRaw.AllianceName
            };
        }

        public static Player GetPlayer(this VillageRaw villageRaw)
        {
            return new Player
            {
                PlayerId = villageRaw.PlayerId,
                Name = villageRaw.PlayerName,
                AllianceId = villageRaw.AllianceId
            };
        }

        public static Village GetVillage(this VillageRaw villageRaw)
        {
            return new Village
            {
                VillageId = villageRaw.VillageId,
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

        public static VillagePopulation GetVillagePopulation(this Village village, DateTime date)
        {
            return new VillagePopulation
            {
                VillageId = village.VillageId,
                Population = village.Population,
                Date = date,
            };
        }

        public static PlayerAlliance GetPlayerAlliance(this Player player, DateTime date)
        {
            return new PlayerAlliance
            {
                PlayerId = player.PlayerId,
                AllianceId = player.AllianceId,
                Date = date,
            };
        }
    }
}