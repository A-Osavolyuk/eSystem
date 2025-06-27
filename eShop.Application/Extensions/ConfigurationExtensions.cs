using Microsoft.Extensions.Configuration;

namespace eShop.Application.Extensions;

public static class ConfigurationExtensions
{
    public static TValue Get<TValue>(this IConfiguration configurationManager, string key)
    {
        var section = configurationManager.GetSection(key);
        
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