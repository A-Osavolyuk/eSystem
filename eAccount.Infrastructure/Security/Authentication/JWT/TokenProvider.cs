namespace eAccount.Infrastructure.Security.Authentication.JWT;

public class TokenProvider()
{
    public string? AccessToken { get; set; }
    public void Clear()
    {
        AccessToken = null;
    }
}