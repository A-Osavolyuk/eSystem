namespace eSystem.Domain.Responses.Auth;

public class GenerateTokenResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}