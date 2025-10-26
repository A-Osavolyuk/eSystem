namespace eAccount.Infrastructure.Security;

public class TokenProvider()
{
    public string? AccessToken { get; set; }
    public void Clear()
    {
        AccessToken = null;
    }
}