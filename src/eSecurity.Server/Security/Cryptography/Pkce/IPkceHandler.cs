namespace eSecurity.Server.Security.Cryptography.Pkce;

public interface IPkceHandler
{
    public bool Verify(string codeChallenge, string codeChallengeMethod, string codeVerifier);
    public string ComputeChallenge(string codeVerifier, string method);
}