using Microsoft.Extensions.Configuration;

namespace eShop.Application.Extensions;

public static class ConfigurationManagerExtensions
{
    public static TValue Get<TValue>(this IConfigurationManager configurationManager, string key)
    {
        var section = configurationManager.GetSection(key);
        var value = section.Get<TValue>()!;
        return value;
    }
}