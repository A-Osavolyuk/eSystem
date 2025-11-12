using eSystem.Core.Common.Exceptions;
using eSystem.Core.Common.Http;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace eSystem.Core.Common.Errors;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling global exception");
        
        if (exception is FailedValidationException failedValidationException)
        {
            _logger.LogInformation("Handled validation exception");
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(
                HttpResponseBuilder.Create()
                    .Failed()
                    .WithMessage(failedValidationException.Message)
                    .Build(), cancellationToken);
        }
        else
        {
            _logger.LogInformation("Handled global exception");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(
                HttpResponseBuilder.Create()
                    .Failed()
                    .WithMessage(exception.Message)
                    .Build(), cancellationToken);
        }
        
        return true;
    }
}