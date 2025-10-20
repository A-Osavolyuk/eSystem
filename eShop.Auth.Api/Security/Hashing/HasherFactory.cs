namespace eShop.Auth.Api.Security.Hashing;

public class HasherFactory(IServiceProvider serviceProvider) : IHasherFactory
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public Hasher Create(HashAlgorithm algorithm)
    {
        return algorithm switch
        {
            HashAlgorithm.Pbkdf2 => serviceProvider.GetRequiredKeyedService<Pbkdf2Hasher>(algorithm),
            _ => throw new NotSupportedException("Unknown hash algorithm")
        };
    }
}