namespace eSecurity.Server.Security.Cryptography.Hashing;

public class HasherFactory(IServiceProvider serviceProvider) : IHasherFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public Hasher Create(HashAlgorithm algorithm) => _serviceProvider.GetRequiredKeyedService<Hasher>(algorithm);
}