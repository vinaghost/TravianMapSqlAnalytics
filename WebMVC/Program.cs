using Features;
using Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;
using WebMVC.Middleware;

namespace WebMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.ConfigureInfrastructure();
            builder.ConfigureFeatures();

            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<ValidateServerMiddleware>();

            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Lax;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<ValidateServerMiddleware>();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.MapDefaultControllerRoute();
            app.UseCookiePolicy();

            app.Run();
        }
    }
}