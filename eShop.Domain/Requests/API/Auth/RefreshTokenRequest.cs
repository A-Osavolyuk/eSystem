namespace eShop.Domain.Requests.API.Auth;

public record RefreshTokenRequest
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
}