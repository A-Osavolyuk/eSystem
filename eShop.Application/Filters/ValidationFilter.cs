using eShop.Domain.Common.API;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eShop.Application.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class ValidationFilter() : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var parameter in context.ActionArguments)
        {
            if (parameter.Value is not null)
            {
                var scope = context.HttpContext.RequestServices.CreateScope();
                var argumentType = parameter.Value.GetType();
                var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
                
                if (scope.ServiceProvider.GetService(validatorType) is IValidator validator)
                {
                    var validationResult = await validator.ValidateAsync(new ValidationContext<object>(parameter.Value));
                    
                    var errors = validationResult.Errors
                        .GroupBy(x => x.PropertyName)
                        .ToDictionary(x => x.Key, 
                            x => x.Select(failure => failure.ErrorMessage).ToList());
                    
                    if (!validationResult.IsValid)
                    {
                        context.Result = new BadRequestObjectResult(
                            new ResponseBuilder()
                                .Failed()
                                .WithResult(errors)
                                .Build());
                        
                        return;
                    }
                }
            }
        }
        
        await next();
    }
}