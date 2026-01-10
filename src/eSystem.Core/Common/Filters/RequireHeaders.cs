using eSystem.Core.Common.Http.Constants;
using eSystem.Core.Common.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eSystem.Core.Common.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class RequireHeaders(params string[] headers) : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var header in headers)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(header, out  var value))
            {
                var error = new Error
                {
                    Code = Results.Errors.OAuth.InvalidRequest,
                    Description = $"Header '{header}' is missing."
                };

                context.Result = new BadRequestObjectResult(error)
                {
                    ContentTypes = { ContentTypes.Application.Json }
                };
                
                return;
            }

            if (string.IsNullOrEmpty(value))
            {
                var error = new Error
                {
                    Code = Results.Errors.OAuth.InvalidRequest,
                    Description = $"Header '{header}' is empty."
                };
                
                context.Result = new BadRequestObjectResult(error)
                {
                    ContentTypes = { ContentTypes.Application.Json }
                };
                
                return;
            }
        }
        
        await next();
    }
}