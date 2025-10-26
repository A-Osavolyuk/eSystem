using Microsoft.Extensions.Hosting;

namespace eSystem.Core.Common.Cache.Redis;

public static class RedisCacheExtensions
{
    public static void AddRedisCache(this IHostApplicationBuilder builder)
    {
        builder.AddRedisClient("redis");
    }
}