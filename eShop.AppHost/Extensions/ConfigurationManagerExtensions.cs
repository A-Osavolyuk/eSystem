using Microsoft.Extensions.Configuration;

namespace eShop.AppHost.Extensions;

public static class ConfigurationManagerExtensions
{
    
    public static TValue GetSectionValue<TValue>(this IConfigurationManager configuration, string key)
    {
        var section = configuration.GetSection(key);
        var value = section.Get<TValue>()!;
        return value;
    }
}