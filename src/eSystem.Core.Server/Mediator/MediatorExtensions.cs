using Microsoft.Extensions.DependencyInjection;

namespace eSystem.Core.Server.Mediator;

public static class MediatorExtensions
{
    public static void AddMediator(this IServiceCollection services,
        Action<MediatorConfigurationBuilder> configurations)
    {
        var builder = new MediatorConfigurationBuilder(services);
        configurations(builder);

        services.AddScoped<IMediator, Mediator>();
    }
}

public sealed class MediatorConfigurationBuilder(IServiceCollection serviceCollection)
{
    private readonly IServiceCollection _serviceCollection = serviceCollection;

    public void AddRequestHandlersFromAssembly<TAssemblyMarker>() where TAssemblyMarker : notnull
    {
        var types = typeof(TAssemblyMarker).Assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false });

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));

            foreach (var @interface in interfaces)
                _serviceCollection.AddTransient(@interface, type);
        }
    }

    public void AddPipelineBehaviorsFromAssembly<TAssemblyMarker>() where TAssemblyMarker : notnull
    {
        var types = typeof(TAssemblyMarker).Assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false });

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));

            foreach (var @interface in interfaces)
                _serviceCollection.AddTransient(@interface, type);
        }
    }

    public void AddRequestHandler<TRequestHandler, TRequest, TResponse>()
        where TRequestHandler : class, IRequestHandler<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
        where TResponse : notnull
    {
        _serviceCollection.AddTransient<IRequestHandler<TRequest, TResponse>, TRequestHandler>();
    }

    public void AddPipelineBehavior<TPipeline, TRequest, TResponse>()
        where TPipeline : class, IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        _serviceCollection.AddTransient<IPipelineBehavior<TRequest, TResponse>, TPipeline>();
    }

    public void AddPipelineBehavior(Type pipelineType)
    {
        if (!pipelineType.IsGenericTypeDefinition)
            throw new ArgumentException(
                $"{pipelineType.Name} must be an open generic type (e.g. typeof(LoggingBehavior<,>)). " +
                $"For closed types use the generic overload instead.");

        var implements = pipelineType
            .GetInterfaces()
            .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));

        if (!implements)
            throw new InvalidOperationException($"{pipelineType.Name} must implement IPipelineBehavior<,>");

        _serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), pipelineType);
    }

    public void AddPipelineFailureHandler<TFailureHandler, TRequest, TResponse>()
        where TFailureHandler : class, IPipelineFailureHandler<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        var serviceType = typeof(IPipelineFailureHandler<TRequest, TResponse>);
        var implementationType = typeof(TFailureHandler);
        var alreadyExists = _serviceCollection.Any(x => x.ServiceType == serviceType && 
            x.ImplementationType == implementationType && x.Lifetime == ServiceLifetime.Transient);
        
        if (alreadyExists)
            throw new InvalidOperationException(
                $"Pipeline failure handler of type {typeof(TFailureHandler).Name} is already registered");
        
        _serviceCollection.AddTransient<IPipelineFailureHandler<TRequest, TResponse>, TFailureHandler>();
    }

    public void AddDefaultPipelineFailureHandler<TFailureHandler>()
        where TFailureHandler : class, IDefaultPipelineFailureHandler
    {
        if (_serviceCollection.Any(x => x.ServiceType == typeof(IDefaultPipelineFailureHandler)))
            throw new InvalidOperationException("Default pipeline failure handler is already registered");
        
        _serviceCollection.AddTransient<IDefaultPipelineFailureHandler, TFailureHandler>();
    }
}