using Microsoft.AspNetCore.Http;

namespace eSystem.Core.Mediator;

public abstract class RequestHandlerBase<TRequest, TResponse>(IHttpContextAccessor httpContextAccessor) 
    : RequestHandlerBase(httpContextAccessor), IRequestHandler<TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

public abstract class RequestHandlerBase<TRequest>(IHttpContextAccessor httpContextAccessor) 
    : RequestHandlerBase(httpContextAccessor), IRequestHandler<TRequest> 
    where TRequest : IRequest
{
    public abstract Task Handle(TRequest request, CancellationToken cancellationToken);
}

public abstract class RequestHandlerBase(IHttpContextAccessor httpContextAccessor)
{
    protected IHttpContextAccessor HttpContextAccessor { get; } = httpContextAccessor;
    protected HttpContext HttpContext => 
        HttpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available");
}