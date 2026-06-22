using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace eSystem.Core.Server.Mediator;

public interface IMediator
{
    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default);
}

public sealed class Mediator(IServiceProvider serviceProvider) : IMediator
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private static readonly ConcurrentDictionary<Type, MethodInfo> MethodCache = new();

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var responseType = typeof(TResponse);
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        var handler = _serviceProvider.GetRequiredService(handlerType);
        var handleMethod = GetMethod(handlerType, nameof(IRequestHandler<,>.Handle));
        
        RequestHandlerDelegate<TResponse> pipeline = () =>
        {
            if (handleMethod is null)
                throw new InvalidOperationException($"No handler found for {handlerType}");

            return (Task<TResponse>)handleMethod.Invoke(handler, [request, cancellationToken])!;
        };

        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var behaviors = _serviceProvider.GetServices(behaviorType).ToList();
        foreach (var behavior in Enumerable.Reverse(behaviors))
        {
            if (behavior is null)
                throw new InvalidOperationException("Invalid behavior");

            var next = pipeline;
            var method = GetMethod(behavior.GetType(), nameof(IPipelineBehavior<,>.Handle));
            if (method is null)
                throw new InvalidOperationException("Invalid behavior");

            pipeline = () => (Task<TResponse>)method.Invoke(behavior, [request, next, cancellationToken])!;
        }

        try
        {
            return await pipeline();
        }
        catch (Exception ex)
        {
            var failureHandlerType = typeof(IPipelineFailureHandler<,>).MakeGenericType(requestType, responseType);
            var failureHandler = _serviceProvider.GetService(failureHandlerType);
            if (failureHandler is not null)
            {
                var method = GetMethod(failureHandlerType, nameof(IPipelineFailureHandler<,>.TryHandle));
                if (method is null)
                    throw new InvalidOperationException($"No pipeline failure handler found for {handlerType}");

                var result = await (Task<PipelineFailureResult<TResponse>>)method.Invoke(
                    failureHandler, [request, ex, cancellationToken])!;

                if (result.Handled)
                    return result.Value!;
            }

            throw;
        }
    }

    private static MethodInfo GetMethod(Type type, string methodName)
    {
        return MethodCache.GetOrAdd(
            type,
            static (t, name) => t.GetMethod(name) ?? throw new InvalidOperationException($"'{name}' not found on {t.Name}"),
            methodName);
    }
}