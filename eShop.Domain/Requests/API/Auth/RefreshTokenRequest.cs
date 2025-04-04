namespace eShop.Domain.Requests.Api.Auth;

public record class RefreshTokenRequest
{
    public string Token { get; set; } = string.Empty;
}