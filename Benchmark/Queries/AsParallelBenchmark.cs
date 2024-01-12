using Benchmark.Setups;
using BenchmarkDotNet.Attributes;
using Core;
using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Benchmark.Queries
{
    /// <summary>
    /// AsParallel does not help much
    /// </summary>
    ///
    [ShortRunJob]
    public class AsParallelBenchmark
    {
        private readonly string _connectionString = SecretAppsettingReader.GetConnectionString("ServerDb");
        private readonly ServerVersion _version;
        private readonly Coordinates _centerCoordinate = new(0, 0);

        [Params(1, 3, 5, 7, 14)]
        public int DateDiff { get; set; }

        public DateTime Date => DateTime.Now.AddDays(-DateDiff);

        public AsParallelBenchmark()
        {
            _version = ServerVersion.AutoDetect(_connectionString);
        }

        [Benchmark]
        public List<VillageContainPopulationHistory> UseParallel()
        {
            using var context = new ServerDbContext(_connectionString, _version);
            return context.Villages
                .Select(x => new
                {
                    x.PlayerId,
                    x.VillageId,
                    x.Name,
                    x.X,
                    x.Y,
                    x.IsCapital,
                    x.Tribe,
                    Populations = x.Populations.OrderByDescending(x => x.Date).Where(x => x.Date >= Date),
                })
                .AsEnumerable()
                .AsParallel()
                .Select(x => new
                {
                    x.PlayerId,
                    x.VillageId,
                    x.Name,
                    x.X,
                    x.Y,
                    x.IsCapital,
                    x.Tribe,
                    x.Populations,
                    Distance = _centerCoordinate.Distance(new Coordinates(x.X, x.Y))
                })
                .Select(x => new VillageContainPopulationHistory(
                    x.PlayerId,
                    x.VillageId,
                    x.Name,
                    x.X,
                    x.Y,
                    x.IsCapital,
                    x.Tribe,
                    x.Distance,
                    x.Populations.Select(x => x.Population).FirstOrDefault() - x.Populations.Select(x => x.Population).LastOrDefault(),
                    x.Populations.Select(x => new PopulationHistoryRecord(x.Population, x.Date))))
                .ToList();
        }

        [Benchmark]
        public List<VillageContainPopulationHistory> NotUseParallel()
        {
            using var context = new ServerDbContext(_connectionString, _version);
            return context.Villages
                .Select(x => new
                {
                    x.PlayerId,
                    x.VillageId,
                    x.Name,
                    x.X,
                    x.Y,
                    x.IsCapital,
                    x.Tribe,
                    Populations = x.Populations.OrderByDescending(x => x.Date).Where(x => x.Date >= Date),
                })
                .AsEnumerable()
                //.AsParallel()
                .Select(x => new
                {
                    x.PlayerId,
                    x.VillageId,
                    x.Name,
                    x.X,
                    x.Y,
                    x.IsCapital,
                    x.Tribe,
                    x.Populations,
                    Distance = _centerCoordinate.Distance(new Coordinates(x.X, x.Y))
                })
                .Select(x => new VillageContainPopulationHistory(
                    x.PlayerId,
                    x.VillageId,
                    x.Name,
                    x.X,
                    x.Y,
                    x.IsCapital,
                    x.Tribe,
                    x.Distance,
                    x.Populations.Select(x => x.Population).FirstOrDefault() - x.Populations.Select(x => x.Population).LastOrDefault(),
                    x.Populations.Select(x => new PopulationHistoryRecord(x.Population, x.Date))))
                .ToList();
        }
    }
}