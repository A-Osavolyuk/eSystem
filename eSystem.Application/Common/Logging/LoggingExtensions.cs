using Microsoft.Extensions.Hosting;

namespace eSystem.Application.Common.Logging;

public static class LoggingExtensions
{
    public static void AddLogging(this IHostApplicationBuilder builder)
    {
        const string key = "Configuration:Logging";
        builder.Logging.AddConfiguration(builder.Configuration.GetSection(key));
    }
}