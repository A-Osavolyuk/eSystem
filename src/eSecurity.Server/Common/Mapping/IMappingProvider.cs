namespace eSecurity.Server.Common.Mapping;

public interface IMappingProvider
{
    public IMapper<TInput, TOutput> CreateMapper<TInput, TOutput>();
    public IAsyncMapper<TInput, TOutput> CreateAsyncMapper<TInput, TOutput>();
}