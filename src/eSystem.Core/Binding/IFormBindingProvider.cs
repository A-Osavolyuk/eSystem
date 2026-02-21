namespace eSystem.Core.Binding;

public interface IFormBindingProvider
{
    public IFormBinder<TOutput> GetRequiredBinder<TOutput>();
}