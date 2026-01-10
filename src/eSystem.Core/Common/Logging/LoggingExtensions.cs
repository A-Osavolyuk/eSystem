using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace eSystem.Core.Common.Logging;

public static class LoggingExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddLogging()
        {
            const string key = "Configuration:Logging";
            builder.Logging.AddConfiguration(builder.Configuration.GetSection(key));
        }
    }
}