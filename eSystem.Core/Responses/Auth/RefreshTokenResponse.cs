namespace eSystem.Core.Responses.Auth;

public class RefreshTokenResponse
{
    public required Guid UserId { get; set; }
    public required string AccessToken { get; set; }
}