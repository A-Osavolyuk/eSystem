namespace eShop.Domain.Requests.API.Auth;

public record class RefreshTokenRequest
{
    public string Token { get; set; } = string.Empty;
}