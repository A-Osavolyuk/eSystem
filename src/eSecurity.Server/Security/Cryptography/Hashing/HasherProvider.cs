namespace eSecurity.Server.Security.Cryptography.Hashing;

public class HasherProvider(IServiceProvider serviceProvider) : IHasherProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IHasher GetHasher(HashAlgorithm algorithm) => _serviceProvider.GetRequiredKeyedService<IHasher>(algorithm);
}