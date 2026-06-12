using Microsoft.Extensions.DependencyInjection;

namespace eSystem.Core.Server.Mediator;

public interface ISender
{
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, 
        CancellationToken cancellationToken = default);
    
    public Task Send(IRequest request, 
        CancellationToken cancellationToken = default);
}

public sealed class Sender(IServiceProvider serviceProvider) : ISender
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, 
        CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        var behaviorType = typeof(IEnumerable<>)
            .MakeGenericType(typeof(IPipelineBehavior<,>))
            .MakeGenericType(requestType, responseType);
        
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var behaviors = ((IEnumerable<object>)_serviceProvider.GetRequiredService(behaviorType)).ToList();

        RequestHandlerDelegate<TResponse> pipeline = () =>
        {
            var method = handlerType.GetMethod(nameof(IRequestHandler<,>.Handle))!;
            if (method is null)
                throw new InvalidOperationException($"No handler found for {handlerType}");
            
            return (Task<TResponse>)method.Invoke(handler, [request, cancellationToken])!;
        };
        
        foreach (var behavior in Enumerable.Reverse(behaviors))
        {
            var next = pipeline;
            var method  = behavior.GetType().GetMethod(nameof(IPipelineBehavior<,>.Handle));
            if (method is null)
                throw new InvalidOperationException("Invalid behavior");
            
            pipeline = () => (Task<TResponse>)method.Invoke(behavior, [request, next, cancellationToken])!;
        }

        return pipeline();
    }

    public Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
        var behaviorType = typeof(IEnumerable<>)
            .MakeGenericType(typeof(IPipelineBehavior<>))
            .MakeGenericType(requestType);
        
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var behaviors = ((IEnumerable<object>)_serviceProvider.GetRequiredService(behaviorType)).ToList();

        RequestHandlerDelegate pipeline = () =>
        {
            var method = handlerType.GetMethod(nameof(IRequestHandler<>.Handle));
            if (method is null) 
                throw new InvalidOperationException($"No handler found for {handlerType}");
            
            return (Task)method.Invoke(handler, [request, cancellationToken])!;
        };
        
        foreach (var behavior in Enumerable.Reverse(behaviors))
        {
            var next = pipeline;
            var method  = behavior.GetType().GetMethod(nameof(IPipelineBehavior<>.Handle));
            if (method is null)
                throw new InvalidOperationException("Invalid behavior");
            
            pipeline = () => (Task)method.Invoke(behavior, [request, next, cancellationToken])!;
        }

        return pipeline();
    }
}