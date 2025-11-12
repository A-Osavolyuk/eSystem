using Microsoft.Extensions.Hosting;

namespace eSystem.Core.Common.Cache.Redis;

public static class RedisCacheExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddRedisCache()
        {
            builder.AddRedisClient("redis");
        }
    }
}