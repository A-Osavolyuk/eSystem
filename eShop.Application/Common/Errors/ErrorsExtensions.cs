using eShop.Application.Middlewares;
using Microsoft.Extensions.Hosting;

namespace eShop.Application.Common.Errors;

public static class ErrorsExtensions
{
    public static void AddExceptionHandler(this IHostApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
    }
}