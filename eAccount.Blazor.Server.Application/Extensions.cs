using Microsoft.Extensions.Hosting;

namespace eAccount.Blazor.Server.Application;

public static class Extensions
{
    public static void AddValidation(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
    }
}