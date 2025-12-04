namespace eSecurity.Server.Security.Cryptography.Hashing;

public class HasherFactory(IServiceProvider serviceProvider) : IHasherFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IHasher CreateHasher(HashAlgorithm algorithm) => _serviceProvider.GetRequiredKeyedService<IHasher>(algorithm);
}