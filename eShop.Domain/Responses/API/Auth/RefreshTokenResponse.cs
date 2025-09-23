namespace eShop.Domain.Responses.API.Auth;

public class RefreshTokenResponse
{
    public required Guid UserId { get; set; }
    public required string AccessToken { get; set; }
}