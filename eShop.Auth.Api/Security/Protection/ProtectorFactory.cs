namespace eShop.Auth.Api.Security.Protection;

public class ProtectorFactory(IServiceProvider serviceProvider) : IProtectorFactory
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public Protector Create(ProtectorType type)
    {
        return type switch
        {
            ProtectorType.Code => serviceProvider.GetRequiredKeyedService<CodeProtector>(type),
            ProtectorType.Secret => serviceProvider.GetRequiredKeyedService<SecretProtector>(type),
            _ => throw new NotSupportedException("Unknown protector type")
        };
    }
}