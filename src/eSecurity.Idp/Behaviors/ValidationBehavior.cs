using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using FluentValidation;

namespace eSecurity.Idp.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IServiceProvider serviceProvider) 
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse> 
    where TResponse : Result
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var validator = _serviceProvider.GetService<IValidator<TRequest>>();
        if (validator is not null)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid && validationResult.Errors.Count > 0)
            {
                var result = Results.ClientError(ClientErrorCode.BadRequest, new Error()
                {
                    Code = ErrorCode.InvalidRequest,
                    Description = validationResult.Errors.First().ErrorMessage
                });

                return (TResponse)result;
            }
        }
        
        var response = await next();
        return response;
    }
}