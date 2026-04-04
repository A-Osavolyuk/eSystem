using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Server.Security.Cryptography.Pkce;

public interface IPkceHandler
{
    public bool Verify(string codeChallenge, ChallengeMethod codeChallengeMethod, string codeVerifier);
    public string ComputeChallenge(string codeVerifier, ChallengeMethod method);
}