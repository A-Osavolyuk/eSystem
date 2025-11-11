namespace eSecurity.Client.Security.Authentication;

public class TokenIdentity
{
    public required string RefreshToken { get; set; }
    public required string IdToken { get; set; }
}