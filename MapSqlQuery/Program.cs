using MainCore;
using MapSqlQuery.Services.Implementations;
using MapSqlQuery.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MapSqlQuery
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                var worldUrl = "ts8.x1.arabics.travian.com";
                var configuration = serviceProvider.GetService<IConfiguration>();
                var connectionString = AppDbContext.GetConnectionString(configuration, worldUrl);
                options.UseMySQL(connectionString);
                //options.EnableSensitiveDataLogging();
            });

            builder.Services.AddSingleton<IDataProvide, DataProvide>();

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