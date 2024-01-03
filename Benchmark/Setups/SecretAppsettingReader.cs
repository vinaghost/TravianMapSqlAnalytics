using Microsoft.Extensions.Configuration;

namespace Benchmark.Setups;

public static class SecretAppsettingReader
{
    public static string GetConnectionString(string dbContext)
    {
        var builder = new ConfigurationBuilder()
            .AddUserSecrets<Program>();
        var configurationRoot = builder.Build();

#pragma warning disable CS8603 // Possible null reference return.
        return configurationRoot.GetConnectionString(dbContext);
#pragma warning restore CS8603 // Possible null reference return.
    }
}