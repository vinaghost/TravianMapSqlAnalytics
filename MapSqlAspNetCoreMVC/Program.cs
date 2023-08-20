using MainCore;
using MapSqlAspNetCoreMVC.Middlewares;
using MapSqlAspNetCoreMVC.Services.Implementations;
using MapSqlAspNetCoreMVC.Services.Interfaces;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace MapSqlAspNetCoreMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services
                .AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts =>
                {
                    opts.ResourcesPath = "Resources";
                })
                .AddDataAnnotationsLocalization();
            builder.Services.AddAuthentication();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                var worldUrl = "ts8.x1.arabics.travian.com";
                var configuration = serviceProvider.GetService<IConfiguration>();
                var connectionString = AppDbContext.GetConnectionString(configuration, worldUrl);
                options.UseMySql(connectionString, ServerVersion.Create(8, 0, 34, Pomelo.EntityFrameworkCore.MySql.Infrastructure.ServerType.MySql));
                //options.EnableSensitiveDataLogging();
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            builder.Services.TryAddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
            builder.Services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

            builder.Services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture("vi-VN");
                options.AddSupportedUICultures("vi-VN", "en-US");
                options.FallBackToParentUICultures = true;

                var requestProvider = options.RequestCultureProviders.OfType<AcceptLanguageHeaderRequestCultureProvider>().First();
                options.RequestCultureProviders.Remove(requestProvider);
            });

            builder.Services.AddScoped<RequestLocalizationCookiesMiddleware>();

            builder.Services.AddSingleton<IDataProvide, DataProvide>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();
            app.UseRequestLocalization();
            app.UseRequestLocalizationCookies();
            app.UseRouting();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}