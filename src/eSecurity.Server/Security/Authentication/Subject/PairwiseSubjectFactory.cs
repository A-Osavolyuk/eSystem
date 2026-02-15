using eSecurity.Server.Security.Cryptography.Hashing;

namespace eSecurity.Server.Security.Authentication.Subject;

public sealed class PairwiseSubjectContext : SubjectContext
{
    public required Guid UserId { get; set; }
    public required string SectorIdentifier { get; set; }
    public required string Salt { get; set; }
}

public sealed class PairwiseSubjectFactory(
    IHasherProvider hasherProvider) : ISubjectFactory<PairwiseSubjectContext>
{
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha256);

    public string CreateSubject(PairwiseSubjectContext context)
    {
        var rawValue = $"{context.UserId}:{context.SectorIdentifier}:{context.Salt}";
        return _hasher.Hash(rawValue);
    }
}