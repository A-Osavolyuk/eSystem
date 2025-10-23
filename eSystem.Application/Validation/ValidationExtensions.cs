using Microsoft.Extensions.Hosting;

namespace eSystem.Application.Validation;

public static class ValidationExtensions
{
    public static void AddValidation<TAssemblyMarker>(this IHostApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<TAssemblyMarker>();
    }
}