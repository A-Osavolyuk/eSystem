using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eSystem.Core.Common.Cache.Redis;

public static class RedisCacheExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddRedisCache()
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("redis");
            });
        }
    }
}