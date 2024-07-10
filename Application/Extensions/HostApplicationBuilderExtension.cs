using Core.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.Extensions
{
    public static class HostApplicationBuilderExtension
    {
        public static IHostApplicationBuilder BindConfiguration(this IHostApplicationBuilder builder)
        {
            builder.Services.Configure<ConnectionStringOption>(options =>
            {
                var config = builder.Configuration.GetConnectionString(ConnectionStringOption.DATASOURCE);
                options.DataSource = config;
            });

            return builder;
        }
    }
}