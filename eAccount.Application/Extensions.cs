using Microsoft.Extensions.Hosting;

namespace eAccount.Application;

public static class Extensions
{
    public static void AddValidation(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
    }
}