namespace eSystem.Auth.Api.Security.Cryptography.Protection;

public class ProtectorFactory(IServiceProvider serviceProvider) : IProtectorFactory
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public Protector Create(ProtectorType type) => serviceProvider.GetRequiredKeyedService<Protector>(type);
}