using Microsoft.Extensions.DependencyInjection;

namespace eSystem.Core.Mediator;

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

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, 
        CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod(nameof(IRequestHandler<,>.Handle));
        if (method is null) throw new NullReferenceException($"No handler found for {handlerType}");

        if (method.Invoke(handler, [request, cancellationToken]) is not Task<TResponse> result) 
            throw new NullReferenceException($"No result found for {handlerType}");

        return await result;
    }

    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod(nameof(IRequestHandler<>.Handle));
        if (method is null) throw new NullReferenceException($"No handler found for {handlerType}");

        if (method.Invoke(handler, [request, cancellationToken]) is not Task result) 
            throw new NullReferenceException($"No result found for {handlerType}");

        await result;
    }
}