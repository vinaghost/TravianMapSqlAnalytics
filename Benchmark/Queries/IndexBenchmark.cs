﻿using BenchmarkDotNet.Attributes;
using Core;
using Core.Entities;
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MySql;

namespace Benchmark.Queries
{
    [ShortRunJob]
    public class IndexBenchmark
    {
        private const string DATABASE_NAME = "mapsql";

        private readonly MySqlContainer _container = new MySqlBuilder()
                                                            .WithImage("mysql:8.0")
                                                            .WithDatabase(DATABASE_NAME)
                                                            .Build();

        private readonly Coordinates _centerCoordinate = new(0, 0);

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

        [GlobalSetup(Target = nameof(WithIndex))]
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
                .Select(x => x.VillageId);

            var populationVillage = context.VillagesPopulations
                .Where(x => distanceVillages.Contains(x.VillageId))
                .Where(x => x.Date >= Date)
                .GroupBy(x => x.VillageId)
                .Select(x => new
                {
                    VillageId = x.Key,
                    Populations = x.OrderByDescending(x => x.Date)
                })
                .AsEnumerable()
                .Select(x => new
                {
                    x.VillageId,
                    ChangePopulation = x.Populations.Select(x => x.Population).FirstOrDefault() - x.Populations.Select(x => x.Population).LastOrDefault(),
                    Populations = x.Populations.Select(x => new PopulationHistoryRecord(x.Population, x.Date))
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