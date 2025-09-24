using Microsoft.Extensions.Hosting;

namespace eShop.Blazor.Server.Application;

public static class Extensions
{
    public static void AddValidation(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
    }
}