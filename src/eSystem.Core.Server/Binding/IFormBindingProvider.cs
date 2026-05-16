namespace eSystem.Core.Server.Binding;

public interface IFormBindingProvider
{
    public IFormBinder<TOutput> GetRequiredBinder<TOutput>();
}