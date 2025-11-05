namespace eSecurity.Security.Authentication.Odic.PKCE;

public interface IPkceHandler
{
    public bool Verify(string codeChallenge, string codeChallengeMethod, string codeVerifier);
    public string ComputeChallenge(string codeVerifier, string method);
}