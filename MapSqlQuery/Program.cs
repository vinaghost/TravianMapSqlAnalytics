using MapSqlQuery.Jobs;
using MapSqlQuery.Services.Implementations;
using MapSqlQuery.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace MapSqlQuery
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var path = Path.Combine(AppContext.BaseDirectory, "app.db");
            var connectionString = $"DataSource={path};Cache=Shared";
            builder.Services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseSqlite(connectionString);
                //options.EnableSensitiveDataLogging();
            });

            builder.Services.AddQuartz(q =>
            {
                q.AddJob<UpdateDatabaseJob>(opts => opts.WithIdentity(UpdateDatabaseJob.Key));
                q.AddTrigger(opts => opts
                    .ForJob(UpdateDatabaseJob.Key)
                    .WithIdentity("UpdateDatabaseJob-trigger")
                    .StartNow()
                );
            });
            builder.Services.AddQuartzHostedService(opt =>
            {
                opt.WaitForJobsToComplete = true;
            });

            builder.Services.AddSingleton<IDataProvide, DataProvide>();
            builder.Services.AddHttpClient<IDataUpdate, DataUpdate>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));

            builder.Services.AddHostedService<StartUpService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}