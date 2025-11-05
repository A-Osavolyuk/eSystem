namespace eSecurity.Security.Authentication.Jwt;

public class TokenProvider()
{
    public string? AccessToken { get; set; }
    public void Clear()
    {
        AccessToken = null;
    }
}