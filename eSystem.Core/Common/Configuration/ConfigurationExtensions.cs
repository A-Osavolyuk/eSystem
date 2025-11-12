using Microsoft.Extensions.Configuration;

namespace eSystem.Core.Common.Configuration;

public static class ConfigurationExtensions
{
    extension(IConfiguration configuration)
    {
        public TValue Get<TValue>(string key)
        {
            var section = configuration.GetSection(key);
        
            if (!section.Exists())
            {
                throw new KeyNotFoundException($"Configuration section '{key}' was not found.");
            }

            var value = section.Get<TValue>();

            if (value == null)
            {
                throw new InvalidOperationException($"Configuration value for key '{key}' could not be found or is null.");
            }

            return value;
        }
    }
}