namespace eShop.Domain.Requests.API.Auth;

public record RefreshTokenRequest
{
    public string Token { get; set; } = string.Empty;
}