using Core.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.Extensions
{
    public static class HostApplicationBuilderExtension
    {
        public static IHostApplicationBuilder BindConfiguration(this IHostApplicationBuilder builder)
        {
            builder.Services.Configure<ConnectionStringOption>(options => options.Value = builder.Configuration[ConnectionStringOption.Position]);
            return builder;
        }
    }
}