using eSecurity.Server.Security.Cryptography.Hashing;

namespace eSecurity.Server.Security.Identity.Claims.Subject;

public class PairwiseSubjectFactory(IHasherFactory hasherFactory) : IPairwiseSubjectFactory
{
    private readonly IHasherFactory _hasherFactory = hasherFactory;

    public string CreateSubject(string userIdentifier, string sectorIdentifier, string salt)
    {
        var hasher = _hasherFactory.CreateHasher(HashAlgorithm.Sha256);
        var input = $"{userIdentifier}|{sectorIdentifier}|{salt}";
        return hasher.Hash(input);
    }
}