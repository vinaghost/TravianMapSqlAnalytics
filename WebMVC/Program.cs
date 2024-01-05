using Core.Extensions;
using WebMVC.Middleware;

namespace WebMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            AddService(builder.Services);

            builder.BindConfiguration();

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

            UseMiddleware(app);

            app.Run();
        }

        private static void AddService(IServiceCollection services)
        {
            services.AddCore();
            services.AddScoped<ValidateServerMiddleware>();
        }

        private static void UseMiddleware(IApplicationBuilder app)
        {
            app.UseMiddleware<ValidateServerMiddleware>();
        }
    }
}