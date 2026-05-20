using eSystem.Core.Server.Binding;

namespace eSecurity.Idp.Common.Binding;

public sealed class FormBindingProvider(IServiceProvider serviceProvider) : IFormBindingProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IFormBinder<TOutput> GetRequiredBinder<TOutput>()
        => _serviceProvider.GetRequiredService<IFormBinder<TOutput>>();
}