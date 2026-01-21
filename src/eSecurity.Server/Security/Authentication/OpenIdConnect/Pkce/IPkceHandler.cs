namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Pkce;

public interface IPkceHandler
{
    public bool Verify(string codeChallenge, string codeChallengeMethod, string codeVerifier);
    public string ComputeChallenge(string codeVerifier, string method);
}