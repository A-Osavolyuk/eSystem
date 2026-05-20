using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eSystem.Core.Server.Bff;

public static class BffExtensions
{
    public static void AddBff(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<BffOptions>(builder.Configuration.GetSection("Bff"));
    }
}