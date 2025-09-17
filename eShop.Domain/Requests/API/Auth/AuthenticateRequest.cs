namespace eShop.Domain.Requests.API.Auth;

public class AuthenticateRequest
{
    public required string RefreshToken { get; set; }
}