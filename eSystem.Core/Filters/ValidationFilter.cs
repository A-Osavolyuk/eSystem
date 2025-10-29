using eSystem.Core.Common.Http;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace eSystem.Core.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class ValidationFilter() : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var parameter in context.ActionArguments)
        {
            if (parameter.Value is not null)
            {
                using var scope = context.HttpContext.RequestServices.CreateScope();
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
                            HttpResponseBuilder.Create()
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