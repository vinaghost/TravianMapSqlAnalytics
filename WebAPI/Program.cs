using Core;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebAPI.Models.Config;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            BindConfiguration(builder);
            // Add services to the container.
            ConfigureServices(builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            //app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDbContext<ServerListDbContext>((serviceProvider, options) =>
            {
                var option = serviceProvider.GetRequiredService<IOptions<DatabaseOption>>();

                var connectionString = $"{GetDatabaseInfo(option)};Database={ServerListDbContext.DATABASE_NAME}";
                options
#if DEBUG
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
#endif
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
        }

        private static void BindConfiguration(WebApplicationBuilder builder)
        {
            builder.Services.Configure<DatabaseOption>(
                builder.Configuration.GetSection(DatabaseOption.Position));
        }

        private static string GetDatabaseInfo(IOptions<DatabaseOption> options)
        {
            var value = options.Value;
            return $"Server={value.Host};Port={value.Port};Uid={value.Username};Pwd={value.Password};";
        }
    }
}