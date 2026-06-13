namespace eSystem.Core.Server.Mediator;

public interface IPipelineBehavior<in TRequest, TResponse> 
    where TRequest : IRequest<TResponse>
{
    public Task Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}

public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();