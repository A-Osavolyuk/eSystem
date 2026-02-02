namespace eSecurity.Server.Common.Mapping;

public interface IMappingProvider
{
    public IMapper<TInput, TOutput> CreateMapper<TInput, TOutput>() 
        where TInput : notnull 
        where TOutput : notnull;
    
    public IAsyncMapper<TInput, TOutput> CreateAsyncMapper<TInput, TOutput>() 
        where TInput : notnull 
        where TOutput : notnull;
}