namespace eSecurity.Server.Common.Mapping;

public interface IMapper<in TInput, out TOutput> 
    where TInput : notnull 
    where TOutput : notnull
{
    public TOutput Map(TInput input);
}