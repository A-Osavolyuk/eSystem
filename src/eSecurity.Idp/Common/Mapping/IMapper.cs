namespace eSecurity.Idp.Common.Mapping;

public interface IMapper<in TInput, out TOutput> 
{
    public TOutput Map(TInput input);
}