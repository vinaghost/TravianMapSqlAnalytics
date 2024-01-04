using Core.Extensions;
using MapSqlAspNetCoreMVC.Extension;
using MapSqlAspNetCoreMVC.Middlewares;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Razor;

namespace MapSqlAspNetCoreMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            ConfigureServices(builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRequestLocalization();
            app.UseRequestLocalizationCookies();
            app.UseRequestServerCookies();
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

        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts =>
                {
                    opts.ResourcesPath = "Resources";
                })
                .AddDataAnnotationsLocalization();

            services.AddLocalizationService();
            services.AddMiddleware();

            services.AddAuthentication();
            services.AddHttpContextAccessor();
            services.AddCore();
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
            });
        }
    }
}