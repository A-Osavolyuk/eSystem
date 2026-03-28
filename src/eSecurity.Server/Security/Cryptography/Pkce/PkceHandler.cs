using System.Security.Cryptography;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using Microsoft.AspNetCore.WebUtilities;

namespace eSecurity.Server.Security.Cryptography.Pkce;

public class PkceHandler : IPkceHandler
{
    public bool Verify(string codeChallenge, ChallengeMethod codeChallengeMethod, string codeVerifier)
    {
        var computedChallenge = ComputeChallenge(codeVerifier, codeChallengeMethod);
        return computedChallenge == codeChallenge;
    }

    public string ComputeChallenge(string codeVerifier, ChallengeMethod method)
    {
        if (method == ChallengeMethod.S256)
        {
            var bytes = Encoding.UTF8.GetBytes(codeVerifier);
            var hash = SHA256.HashData(bytes);
            return WebEncoders.Base64UrlEncode(hash);
        }
        
        return codeVerifier;
    }
}