using eShop.Blazor.UI;
using Microsoft.Extensions.Hosting;

namespace eShop.Blazor.Application;

public static class Extensions
{
    public static void AddValidation(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
    }
}