using System.Security.Cryptography;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using Microsoft.AspNetCore.WebUtilities;

namespace eSecurity.Server.Security.Cryptography.Pkce;

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
            return WebEncoders.Base64UrlEncode(hash);
        }
        
        return codeVerifier;
    }
}