namespace eSecurity.Server.Common.Mapping;

public sealed class MappingProvider(IServiceProvider serviceProvider) : IMappingProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IMapper<TInput, TOutput> CreateMapper<TInput, TOutput>() 
        where TInput : notnull 
        where TOutput : notnull
        => _serviceProvider.GetRequiredService<IMapper<TInput, TOutput>>();

    public IAsyncMapper<TInput, TOutput> CreateAsyncMapper<TInput, TOutput>()
        where TInput : notnull
        where TOutput : notnull
        => _serviceProvider.GetRequiredService<IAsyncMapper<TInput, TOutput>>();
}