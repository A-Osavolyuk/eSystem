namespace eSecurity.Client.Security.Authentication.Oidc.Token;

public class TokenProvider
{
    public string? AccessToken { get; set; }
    public string? IdToken { get; set; }

    public void Clear()
    {
        AccessToken = null;
        IdToken = null;
    }
}