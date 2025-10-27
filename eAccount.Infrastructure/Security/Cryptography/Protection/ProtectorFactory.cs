using eSystem.Core.Security.Cryptography.Protection;

namespace eAccount.Infrastructure.Security.Cryptography.Protection;

public class ProtectorFactory(IServiceProvider serviceProvider) : IProtectorFactory
{
    private readonly IServiceProvider serviceProvider = serviceProvider;
    public IProtector Create(string purpose) => serviceProvider.GetRequiredKeyedService<IProtector>(purpose);
}