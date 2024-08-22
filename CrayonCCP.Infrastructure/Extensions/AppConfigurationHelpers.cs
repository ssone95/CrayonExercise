using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace CrayonCCP.Infrastructure.Extensions;

public static class AppConfigurationHelpers
{
    public static IConfigurationBuilder SetupConfiguration(this WebApplicationBuilder builder)
    {
        builder.Configuration.Sources.Clear();
        return builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: false,
                reloadOnChange: true)
            .AddEnvironmentVariables();
    }
}