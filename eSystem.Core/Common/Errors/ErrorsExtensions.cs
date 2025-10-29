using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eSystem.Core.Common.Errors;

public static class ErrorsExtensions
{
    public static void AddExceptionHandler(this IHostApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
    }
}