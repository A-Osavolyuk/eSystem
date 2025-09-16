namespace eShop.Domain.Responses.API.Auth;

public class AuthenticateResponse
{
    public required Guid UserId { get; set; }
    public required string AccessToken { get; set; }
}