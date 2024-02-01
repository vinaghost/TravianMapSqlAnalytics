using BenchmarkDotNet.Attributes;
using Core;
using Core.Features.GetVillageContainsPopulationHistory;
using Core.Features.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MySql;
using X.PagedList;

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
    //
    // but if you limit, in my case is distance < 40, it suprises me
    //
    //| Method            | DateDiff | Mean       | Error        | StdDev      | Ratio | RatioSD |
    //|------------------ |--------- |-----------:|-------------:|------------:|------:|--------:|
    //| WithoutIndex      | 1        |   469.6 ms |     59.34 ms |     3.25 ms |  1.00 |    0.00 |
    //| WithIndex         | 1        | 1,986.8 ms |    763.06 ms |    41.83 ms |  4.23 |    0.09 |
    //| WithIndexAndGroup | 1        |   162.4 ms |    180.39 ms |     9.89 ms |  0.35 |    0.02 |
    //|                   |          |            |              |             |       |         |
    //| WithoutIndex      | 3        | 1,033.3 ms |    403.08 ms |    22.09 ms |  1.00 |    0.00 |
    //| WithIndex         | 3        | 2,656.3 ms |  4,186.91 ms |   229.50 ms |  2.57 |    0.20 |
    //| WithIndexAndGroup | 3        |   253.3 ms |    137.26 ms |     7.52 ms |  0.25 |    0.00 |
    //|                   |          |            |              |             |       |         |
    //| WithoutIndex      | 5        | 1,516.2 ms |    419.25 ms |    22.98 ms |  1.00 |    0.00 |
    //| WithIndex         | 5        | 3,367.0 ms |  2,234.07 ms |   122.46 ms |  2.22 |    0.05 |
    //| WithIndexAndGroup | 5        |   327.8 ms |     99.44 ms |     5.45 ms |  0.22 |    0.01 |
    //|                   |          |            |              |             |       |         |
    //| WithoutIndex      | 7        | 2,070.2 ms |    651.73 ms |    35.72 ms |  1.00 |    0.00 |
    //| WithIndex         | 7        | 3,823.3 ms |  1,538.58 ms |    84.33 ms |  1.85 |    0.03 |
    //| WithIndexAndGroup | 7        |   397.9 ms |    547.79 ms |    30.03 ms |  0.19 |    0.01 |
    //|                   |          |            |              |             |       |         |
    //| WithoutIndex      | 14       | 4,859.1 ms | 19,725.81 ms | 1,081.24 ms |  1.00 |    0.00 |
    //| WithIndex         | 14       | 5,017.6 ms |  1,874.32 ms |   102.74 ms |  1.07 |    0.26 |
    //| WithIndexAndGroup | 14       |   821.3 ms |    656.69 ms |    36.00 ms |  0.18 |    0.04 |
    //
    // apply ipagedlist, more date make the new method works better than old one
    //
    //| Method            | DateDiff | Mean       | Error       | StdDev    | Ratio | RatioSD |
    //|------------------ |--------- |-----------:|------------:|----------:|------:|--------:|
    //| WithoutIndex      | 1        |   626.4 ms |    100.4 ms |   5.50 ms |  1.00 |    0.00 |
    //| WithIndex         | 1        | 3,885.1 ms |    887.3 ms |  48.63 ms |  6.20 |    0.07 |
    //| WithIndexAndGroup | 1        | 2,166.8 ms |    192.5 ms |  10.55 ms |  3.46 |    0.02 |
    //|                   |          |            |             |           |       |         |
    //| WithoutIndex      | 3        | 1,473.0 ms |    111.7 ms |   6.12 ms |  1.00 |    0.00 |
    //| WithIndex         | 3        | 4,491.2 ms |  2,905.5 ms | 159.26 ms |  3.05 |    0.10 |
    //| WithIndexAndGroup | 3        | 3,100.6 ms | 15,629.6 ms | 856.71 ms |  2.10 |    0.57 |
    //|                   |          |            |             |           |       |         |
    //| WithoutIndex      | 5        | 2,848.5 ms | 14,693.3 ms | 805.39 ms |  1.00 |    0.00 |
    //| WithIndex         | 5        | 5,086.0 ms |  2,382.0 ms | 130.56 ms |  1.86 |    0.42 |
    //| WithIndexAndGroup | 5        | 3,267.1 ms | 16,762.4 ms | 918.81 ms |  1.15 |    0.01 |
    //|                   |          |            |             |           |       |         |
    //| WithoutIndex      | 7        | 5,436.5 ms | 12,353.2 ms | 677.12 ms |  1.00 |    0.00 |
    //| WithIndex         | 7        | 5,825.8 ms |    210.0 ms |  11.51 ms |  1.08 |    0.14 |
    //| WithIndexAndGroup | 7        | 3,926.8 ms | 15,360.1 ms | 841.94 ms |  0.72 |    0.07 |
    //|                   |          |            |             |           |       |         |
    //| WithoutIndex      | 14       | 7,847.2 ms |  1,363.8 ms |  74.75 ms |  1.00 |    0.00 |
    //| WithIndex         | 14       | 7,727.8 ms |    530.8 ms |  29.10 ms |  0.98 |    0.01 |
    //| WithIndexAndGroup | 14       | 4,656.4 ms |    815.9 ms |  44.72 ms |  0.59 |    0.00 |
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

        [GlobalSetup(Targets = [nameof(WithoutIndex), nameof(WithoutIndexAndGroup)])]
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
        public async Task<IPagedList<VillageContainPopulationHistory>> WithoutIndex()
        {
            using var context = new ServerDbContext(_container.GetConnectionString(), DATABASE_NAME);
            return await Get(context);
        }

        [Benchmark]
        public async Task<IPagedList<VillageContainPopulationHistory>> WithIndex()
        {
            using var context = new ServerDbWithIndexContext(_container.GetConnectionString(), DATABASE_NAME);
            return await Get(context);
        }

        [Benchmark]
        public async Task<IPagedList<VillageContainPopulationHistory>> WithoutIndexAndGroup()
        {
            using var context = new ServerDbContext(_container.GetConnectionString(), DATABASE_NAME);
            return await GetGroup(context);
        }

        [Benchmark]
        public async Task<IPagedList<VillageContainPopulationHistory>> WithIndexAndGroup()
        {
            using var context = new ServerDbWithIndexContext(_container.GetConnectionString(), DATABASE_NAME);
            return await GetGroup(context);
        }

        public async Task<IPagedList<VillageContainPopulationHistory>> GetGroup(ServerDbContext context)
        {
            var distanceVillages = context.Villages
                .Select(x => new
                {
                    VillageId = x.Id,
                    x.X,
                    x.Y,
                })
                .AsEnumerable()
                .Select(x => new
                {
                    x.VillageId,
                    Distance = _centerCoordinate.Distance(new Coordinates(x.X, x.Y))
                })
                .Select(x => x.VillageId);

            var populationVillage = context.VillagePopulationHistory
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

            return await context.Villages
                .Where(x => populationVillage.Keys.Contains(x.Id))
                .Select(x => new
                {
                    x.PlayerId,
                    VillageId = x.Id,
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
                .ToPagedListAsync(1, 20);
        }

        public async Task<IPagedList<VillageContainPopulationHistory>> Get(ServerDbContext context)
        {
            return await context.Villages
               .Select(x => new
               {
                   x.PlayerId,
                   VillageId = x.Id,
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
               .ToPagedListAsync(1, 20);
        }
    }

    internal class ServerDbWithIndexContext : ServerDbContext
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

            modelBuilder.Entity<Core.Entities.VillagePopulationHistory>()
                .HasIndex(x => x.Date);
        }
    }
}