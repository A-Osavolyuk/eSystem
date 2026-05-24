using eSecurity.Idp.Security.Cryptography.Hashing;

namespace eSecurity.Idp.Security.Identity.Subject;

public class PairwiseSubjectFactory(IHasherProvider hasherProvider) : IPairwiseSubjectFactory
{
    private readonly IHasherProvider _hasherProvider = hasherProvider;

    public string CreateSubject(string userIdentifier, string sectorIdentifier, string salt)
    {
        var hasher = _hasherProvider.GetHasher(HashAlgorithm.Sha256);
        var input = $"{userIdentifier}|{sectorIdentifier}|{salt}";
        return hasher.Hash(input);
    }
}