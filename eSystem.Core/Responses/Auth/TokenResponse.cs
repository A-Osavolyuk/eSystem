namespace eSystem.Core.Responses.Auth;

public class TokenResponse
{
    public required string AccessToken { get; set; }
    public required string TokenType { get; set; }
    public required int ExpiresIn { get; set; }
    public required string RefreshToken { get; set; }
}