using eSystem.Core.Common.Exceptions;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace eSystem.Core.Http.Errors;

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
            await httpContext.Response.WriteAsJsonAsync(new Error()
            {
                Code = ErrorTypes.OAuth.ServerError,
                Description = failedValidationException.Message
            }, cancellationToken);
        }
        else
        {
            _logger.LogInformation("Handled global exception");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(new Error()
            {
                Code = ErrorTypes.OAuth.ServerError,
                Description = exception.Message
            }, cancellationToken);
        }
        
        return true;
    }
}