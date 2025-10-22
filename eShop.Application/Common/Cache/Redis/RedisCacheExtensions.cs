using Microsoft.Extensions.Hosting;

namespace eShop.Application.Common.Cache.Redis;

public static class RedisCacheExtensions
{
    public static void AddRedisCache(this IHostApplicationBuilder builder)
    {
        builder.AddRedisClient("redis");
    }
}