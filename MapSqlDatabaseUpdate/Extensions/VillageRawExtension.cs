using Core.Models;
using MapSqlDatabaseUpdate.Models;

namespace MapSqlDatabaseUpdate.Extensions
{
    public static class VillageRawExtension
    {
        public static AllianceEntity GetAlliace(this VillageRaw villageRaw)
        {
            return new AllianceEntity
            {
                AllianceId = villageRaw.AllianceId,
                Name = villageRaw.AllianceName
            };
        }

        public static PlayerEntity GetPlayer(this VillageRaw villageRaw)
        {
            return new PlayerEntity
            {
                PlayerId = villageRaw.PlayerId,
                Name = villageRaw.PlayerName,
                AllianceId = villageRaw.AllianceId
            };
        }

        public static VillageEntity GetVillage(this VillageRaw villageRaw)
        {
            return new VillageEntity
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

        public static VillagePopulation GetVillagePopulation(this VillageEntity village, DateTime date)
        {
            return new VillagePopulation
            {
                VillageId = village.VillageId,
                Population = village.Population,
                Date = date,
            };
        }

        public static PlayerAlliance GetPlayerAlliance(this PlayerEntity player, DateTime date)
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