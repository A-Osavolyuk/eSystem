using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace eShop.Application.Middlewares;

public class  GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling global exception");
        
        if (exception is FailedValidationException failedValidationException)
        {
            logger.LogInformation("Handled validation exception");
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(
                new ResponseBuilder()
                    .Failed()
                    .WithMessage(failedValidationException.Message)
                    .Build(), cancellationToken);
        }
        else
        {
            logger.LogInformation("Handled global exception");
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(
                new ResponseBuilder()
                    .Failed()
                    .WithMessage(exception.Message)
                    .Build(), cancellationToken);
        }
        
        return true;
    }
}