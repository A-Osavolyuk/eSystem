using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace eSystem.Core.Common.Error;

public static class ErrorExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        public void AddExceptionHandling<THandler>() where THandler : class, IExceptionHandler
        {
            builder.Services.AddExceptionHandler<THandler>();
            builder.Services.AddProblemDetails();
        }
    }
}