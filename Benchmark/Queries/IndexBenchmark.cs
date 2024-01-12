using BenchmarkDotNet.Attributes;
using Core;
using Core.Entities;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MySql;

namespace Benchmark.Queries
{
    /// <summary>
    //| Method            | DateDiff | Mean       | Error       | StdDev    | Ratio | RatioSD |
    //|------------------ |--------- |-----------:|------------:|----------:|------:|--------:|
    //| WithoutIndex      | 1        |   508.2 ms |    328.9 ms |  18.03 ms |  1.00 |    0.00 |
    //| WithIndex         | 1        | 1,904.5 ms |    876.9 ms |  48.07 ms |  3.75 |    0.07 |
    //| WithIndexAndGroup | 1        | 2,109.5 ms |    334.6 ms |  18.34 ms |  4.15 |    0.17 |
    //|                   |          |            |             |           |       |         |
    //| WithoutIndex      | 3        | 1,065.7 ms |    481.0 ms |  26.37 ms |  1.00 |    0.00 |
    //| WithIndex         | 3        | 2,217.4 ms |    461.5 ms |  25.30 ms |  2.08 |    0.04 |
    //| WithIndexAndGroup | 3        | 2,441.6 ms |  1,316.7 ms |  72.17 ms |  2.29 |    0.04 |
    //|                   |          |            |             |           |       |         |
    //| WithoutIndex      | 5        | 1,690.2 ms |    124.9 ms |   6.85 ms |  1.00 |    0.00 |
    //| WithIndex         | 5        | 2,812.5 ms |  1,024.1 ms |  56.14 ms |  1.66 |    0.04 |
    //| WithIndexAndGroup | 5        | 3,103.9 ms | 16,803.5 ms | 921.05 ms |  1.84 |    0.54 |
    //|                   |          |            |             |           |       |         |
    //| WithoutIndex      | 7        | 2,075.6 ms |    661.0 ms |  36.23 ms |  1.00 |    0.00 |
    //| WithIndex         | 7        | 3,110.3 ms |  1,512.4 ms |  82.90 ms |  1.50 |    0.06 |
    //| WithIndexAndGroup | 7        | 3,199.9 ms | 11,980.3 ms | 656.68 ms |  1.54 |    0.34 |
    //|                   |          |            |             |           |       |         |
    //| WithoutIndex      | 14       | 4,203.7 ms |    872.5 ms |  47.83 ms |  1.00 |    0.00 |
    //| WithIndex         | 14       | 4,139.1 ms |    820.3 ms |  44.96 ms |  0.98 |    0.02 |
    //| WithIndexAndGroup | 14       | 4,868.8 ms |  6,359.7 ms | 348.60 ms |  1.16 |    0.08 |
    /// </summary>
    [ShortRunJob]
    public class IndexBenchmark
    {
        private const string DATABASE_NAME = "mapsql";

        private readonly MySqlContainer _container = new MySqlBuilder()
                                                            .WithImage("mysql:8.0")
                                                            .WithDatabase(DATABASE_NAME)
                                                            .Build();

        private readonly Coordinates _centerCoordinate = new(50, 50);

        [Params(1, 3, 5, 7, 14)]
        public int DateDiff { get; set; }

        public DateTime Date => DateTime.Now.AddDays(-DateDiff);

        [GlobalSetup(Target = nameof(WithoutIndex))]
        public async Task SetupWithoutIndex()
        {
            var sql = await File.ReadAllTextAsync("Queries/data.sql");

            await _container.StartAsync();
            using (var context = new ServerDbContext(_container.GetConnectionString(), DATABASE_NAME))
            {
                await context.Database.EnsureCreatedAsync();
            }
            await _container.ExecScriptAsync(sql);
        }

        [GlobalSetup(Targets = [nameof(WithIndex), nameof(WithIndexAndGroup)])]
        public async Task SetupWithIndex()
        {
            var sql = await File.ReadAllTextAsync("Queries/data.sql");

            await _container.StartAsync();
            using (var context = new ServerDbWithIndexContext(_container.GetConnectionString(), DATABASE_NAME))
            {
                await context.Database.EnsureCreatedAsync();
            }
            await _container.ExecScriptAsync(sql);
        }

        [Benchmark(Baseline = true)]
        public List<VillageContainPopulationHistory> WithoutIndex()
        {
            using var context = new ServerDbContext(_container.GetConnectionString(), DATABASE_NAME);
            return Get(context);
        }

        [Benchmark]
        public List<VillageContainPopulationHistory> WithIndex()
        {
            using var context = new ServerDbWithIndexContext(_container.GetConnectionString(), DATABASE_NAME);
            return Get(context);
        }

        [Benchmark]
        public List<VillageContainPopulationHistory> WithIndexAndGroup()
        {
            using var context = new ServerDbWithIndexContext(_container.GetConnectionString(), DATABASE_NAME);
            var distanceVillages = context.Villages
                .Select(x => new
                {
                    x.VillageId,
                    x.X,
                    x.Y,
                })
                .AsEnumerable()
                .Select(x => new
                {
                    x.VillageId,
                    Distance = _centerCoordinate.Distance(new Coordinates(x.X, x.Y))
                })
                .Where(x => x.Distance < 40)
                .Select(x => x.VillageId);

            var populationVillage = context.VillagesPopulations
                .Where(x => distanceVillages.Contains(x.VillageId))
                .Where(x => x.Date >= Date)
                .GroupBy(x => x.VillageId)
                .Select(x => new
                {
                    VillageId = x.Key,
                    Populations = x.OrderByDescending(x => x.Date).Select(x => new PopulationHistoryRecord(x.Population, x.Date))
                })
                .AsEnumerable()
                .Select(x => new
                {
                    x.VillageId,
                    ChangePopulation = x.Populations.Select(x => x.Amount).FirstOrDefault() - x.Populations.Select(x => x.Amount).LastOrDefault(),
                    x.Populations
                })
                .ToDictionary(x => x.VillageId, x => new { x.ChangePopulation, x.Populations });

            return context.Villages
                .Where(x => populationVillage.Keys.Contains(x.VillageId))
                .Select(x => new
                {
                    x.PlayerId,
                    x.VillageId,
                    x.Name,
                    x.X,
                    x.Y,
                    x.IsCapital,
                    x.Tribe,
                })
                .AsEnumerable()
                .Select(x =>
                {
                    var population = populationVillage[x.VillageId];
                    return new VillageContainPopulationHistory(
                       x.PlayerId,
                       x.VillageId,
                       x.Name,
                       x.X,
                       x.Y,
                       x.IsCapital,
                       x.Tribe,
                       _centerCoordinate.Distance(new Coordinates(x.X, x.Y)),
                       population.ChangePopulation,
                       population.Populations);
                })
                .ToList();
        }

        public List<VillageContainPopulationHistory> Get(ServerDbContext context)
        {
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
               .Where(x => x.Distance < 40)
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

    public class ServerDbWithIndexContext : ServerDbContext
    {
        [ActivatorUtilitiesConstructor]
        public ServerDbWithIndexContext(DbContextOptions<ServerDbContext> options) : base(options)
        {
        }

        public ServerDbWithIndexContext(string connectionStringWithoutDatabase, string database) : base(connectionStringWithoutDatabase, database)
        {
        }

        public ServerDbWithIndexContext(string connectionString, ServerVersion version) : base(connectionString, version)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<VillagePopulation>()
                .HasIndex(x => x.Date);
        }
    }
}