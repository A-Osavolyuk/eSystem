using eSecurity.Server.Security.Cryptography.Hashing;

namespace eSecurity.Server.Security.Authentication.Subject.Pairwise;

public sealed class PairwiseSubjectFactoryContext : SubjectFactoryContext
{
    public required Guid UserId { get; set; }
    public required string SectorIdentifier { get; set; }
    public required string Salt { get; set; }
}

public sealed class PairwiseSubjectFactory(
    IHasherProvider hasherProvider) : ISubjectFactory<PairwiseSubjectFactoryContext>
{
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha256);

    public string CreateSubject(PairwiseSubjectFactoryContext factoryContext)
    {
        var rawValue = $"{factoryContext.UserId}:{factoryContext.SectorIdentifier}:{factoryContext.Salt}";
        return _hasher.Hash(rawValue);
    }
}