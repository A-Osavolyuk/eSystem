using eSecurity.Server.Security.Cryptography.Hashing;

namespace eSecurity.Server.Security.Identity.Claims.Subject;

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