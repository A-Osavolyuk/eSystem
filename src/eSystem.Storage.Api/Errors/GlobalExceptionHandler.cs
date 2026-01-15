using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using Microsoft.AspNetCore.Diagnostics;

namespace eSystem.Storage.Api.Errors;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling global exception");

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new Error()
        {
            Code = ErrorTypes.OAuth.ServerError,
            Description = exception.Message
        }, cancellationToken);

        return true;
    }
}