namespace eSystem.Auth.Api.Security.Cryptography.Hashing;

public class HasherFactory(IServiceProvider serviceProvider) : IHasherFactory
{
    private readonly IServiceProvider serviceProvider = serviceProvider;

    public Hasher Create(HashAlgorithm algorithm) => serviceProvider.GetRequiredKeyedService<Hasher>(algorithm);
}