using System.Security.Cryptography;
using eSystem.Auth.Api.Security.Cryptography.Encoding;

namespace eSystem.Auth.Api.Security.Authentication.SSO.PKCE;

public class PkceHandler : IPkceHandler
{
    public bool Verify(string codeChallenge, string codeChallengeMethod, string codeVerifier)
    {
        var computedChallenge = ComputeChallenge(codeVerifier, codeChallengeMethod);
        return computedChallenge == codeChallenge;
    }

    public string ComputeChallenge(string codeVerifier, string method)
    {
        if (method == ChallengeMethods.S256)
        {
            var bytes = Encoding.UTF8.GetBytes(codeVerifier);
            var hash = SHA256.HashData(bytes);
            return Base64Url.Encode(hash);
        }
        
        return codeVerifier;
    }
}