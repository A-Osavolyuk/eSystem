using eSystem.Core.Primitives;

namespace eSecurity.Idp.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IServiceProvider serviceProvider) 
    : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>, IRequest<Result>
    where TResponse : Result
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var validator = _serviceProvider.GetService<IRequestValidator<TRequest>>();
        if (validator is not null)
        {
            var validationResult = await validator.Validate(request, cancellationToken);
            if (!validationResult.Succeeded)
                return (TResponse)validationResult;
        }
        
        var response = await next();
        return response;
    }
}