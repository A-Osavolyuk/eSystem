using Microsoft.Extensions.DependencyInjection;

namespace eSystem.Core.Server.Mediator;

public static class MediatorExtensions
{
    public static void AddMediator(this IServiceCollection services,
        Action<MediatorConfigurationBuilder> configurations)
    {
        var builder = new MediatorConfigurationBuilder(services);
        configurations(builder);
    }
}

public sealed class MediatorConfigurationBuilder(IServiceCollection serviceCollection)
{
    private readonly IServiceCollection _serviceCollection = serviceCollection;

    public void FromAssembly<TAssemblyMarker>() where TAssemblyMarker : notnull
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

        _serviceCollection.AddScoped<ISender, Sender>();
    }

    public void AddRequestHandler<TRequestHandler, TRequest, TResponse>()
        where TRequestHandler : class, IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : notnull
    {
        _serviceCollection.AddTransient<IRequestHandler<TRequest, TResponse>, TRequestHandler>();
    }

    public void AddPipelineBehavior<TPipeline, TRequest, TResponse>()
        where TPipeline : class, IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
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
}