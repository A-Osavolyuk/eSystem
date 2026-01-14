using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eSystem.Core.Http.Errors;

public static class ErrorsExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddExceptionHandler()
        {
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
        }
    }
}